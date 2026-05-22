import fs from "node:fs/promises";
import { FileBlob, SpreadsheetFile } from "@oai/artifact-tool";

const inputPath = "C:/Users/hugop/Downloads/Plantilla_B (2).xlsx";
const outputDir = "C:/Users/hugop/source/repos/Hugotemans0/Proyecto-DDS-PSW/outputs/earned_value";
const outputPath = `${outputDir}/Plantilla_B_completada_valor_ganado.xlsx`;

const activities = [
  ["A", "-", 1, 0, 1, 1, "Analista", 200, 192, 1],
  ["B", "A", 3, 1, 4, 2, "Analista", 1200, 1056, 1],
  ["C", "A", 4, 1, 5, 2, "Analista", 1600, 736, 1],
  ["D", "A", 2, 1, 3, 1, "Programador", 300, 246, 1],
  ["E", "B,C", 1, 5, 6, 2, "Analista", 400, 137.6, 0.4],
  ["F", "C", 4, 5, 9, 2, "Programador", 1200, 342, 0.3],
  ["G", "C,D", 4, 5, 9, 2, "Programador", 1200, 201.6, 0.2],
  ["H", "E,F,G", 1, 9, 10, 2, "Programador", 300, 0, 0],
];

const weeks = Array.from({ length: 11 }, (_, i) => i + 1);
const statusWeek = 6;

const input = await FileBlob.load(inputPath);
const workbook = await SpreadsheetFile.importXlsx(input);
const sheet = workbook.worksheets.getItem("Calculos Flujo de Caja");
const unused = workbook.worksheets.getItem("Caminocrítico");

function setFormat(range, format) {
  Object.assign(range.format, format);
}

function cellName(colIndex, row) {
  let n = colIndex;
  let s = "";
  while (n > 0) {
    const r = (n - 1) % 26;
    s = String.fromCharCode(65 + r) + s;
    n = Math.floor((n - 1) / 26);
  }
  return `${s}${row}`;
}

function ganttColumnForWeek(week) {
  return 15 + week; // P = week 1
}

function fillCell(row, week, fill, value = "") {
  const address = cellName(ganttColumnForWeek(week), row);
  const range = sheet.getRange(address);
  range.values = [[value]];
  range.format.fill = fill;
  range.format.font = { color: "#111827", bold: true };
  range.format.horizontalAlignment = "center";
  range.format.verticalAlignment = "center";
}

function colorScheduled(row, start, finish, fill) {
  for (const week of weeks) {
    if (week > start && week <= finish) {
      fillCell(row, week, fill);
    }
  }
}

function colorProgress(row, start, finish, completePct) {
  const progressPoint = start + (finish - start) * completePct;
  for (const week of weeks) {
    const range = sheet.getRange(cellName(ganttColumnForWeek(week), row));
    if (week > start && week <= progressPoint) {
      range.format.fill = "#22C55E";
    } else if (completePct > 0 && week - 1 < progressPoint && week >= progressPoint) {
      range.format.fill = "#BBF7D0";
      range.values = [[`${Math.round(completePct * 100)}%`]];
      range.format.font = { color: "#166534", bold: true };
    } else if (week > start && week <= finish) {
      range.format.fill = "#DBEAFE";
    }
    range.format.horizontalAlignment = "center";
    range.format.verticalAlignment = "center";
  }
}

function markStatusLine(row, week, symbol = "|") {
  const range = sheet.getRange(cellName(ganttColumnForWeek(week), row));
  range.values = [[symbol]];
  range.format.font = { color: "#DC2626", bold: true, size: 13 };
  range.format.horizontalAlignment = "center";
  range.format.verticalAlignment = "center";
}

await fs.mkdir(outputDir, { recursive: true });

sheet.getRange("A1:AZ120").clear({ applyTo: "all" });
sheet.getRange("B2:N10").values = [[
  "Actividad",
  "Pred",
  "Duración",
  "Inicio temp",
  "Fin temp.",
  "Cantidad",
  "Recurso",
  "Coste planificado",
  "Coste real",
  "Finalizada al",
  "Plan % s6",
  "VP act.",
  "VG act.",
], ...activities];

sheet.getRange("L3:L10").formulas = activities.map((_, i) => {
  const row = i + 3;
  return [`=MAX(0,MIN(1,($C$13-E${row})/(F${row}-E${row})))`];
});
sheet.getRange("M3:M10").formulas = activities.map((_, i) => {
  const row = i + 3;
  return [`=I${row}*L${row}`];
});
sheet.getRange("N3:N10").formulas = activities.map((_, i) => {
  const row = i + 3;
  return [`=I${row}*K${row}`];
});

setFormat(sheet.getRange("B2:N2"), {
  fill: "#1F4E78",
  font: { color: "#FFFFFF", bold: true },
  horizontalAlignment: "center",
  verticalAlignment: "center",
  wrapText: true,
  borders: { preset: "outside", style: "thin", color: "#1F2937" },
});
setFormat(sheet.getRange("B3:N10"), {
  borders: { preset: "outside", style: "thin", color: "#CBD5E1" },
  verticalAlignment: "center",
});
sheet.getRange("D3:G10").format.horizontalAlignment = "center";
sheet.getRange("I3:J10").format.numberFormat = "0.000";
sheet.getRange("K3:L10").format.numberFormat = "0%";
sheet.getRange("M3:N10").format.numberFormat = "0.000";

sheet.getRange("B13:E13").values = [["Semana de control", statusWeek, "Fecha de estado", "Fin de semana 6"]];
setFormat(sheet.getRange("B13:E13"), {
  fill: "#E0F2FE",
  font: { bold: true, color: "#0F172A" },
  borders: { preset: "outside", style: "thin", color: "#38BDF8" },
  horizontalAlignment: "center",
});

sheet.getRange("B15:E25").clear({ applyTo: "all" });
sheet.getRange("B15:E25").values = [
  ["Indicador", "Cálculo en la hoja", "Valor", "Respuesta"],
  ["BAC", "SUM(I3:I10)", null, null],
  ["VP", "SUM(M3:M10)", null, null],
  ["CR", "SUM(J3:J10)", null, null],
  ["VG", "SUM(N3:N10)", null, null],
  ["CV", "VG - CR", null, null],
  ["CPI", "VG / CR", null, null],
  ["SV", "VG - VP", null, null],
  ["SPI", "VG / VP", null, null],
  ["ETC", "BAC - VG; variación no repetible", null, null],
  ["EAC", "CR + ETC", null, null],
];
sheet.getRange("D16:D25").formulas = [
  ["=ROUND(SUM(I3:I10),3)"],
  ["=ROUND(SUM(M3:M10),3)"],
  ["=ROUND(SUM(J3:J10),3)"],
  ["=ROUND(SUM(N3:N10),3)"],
  ["=ROUND(D19-D18,3)"],
  ["=ROUND(D19/D18,3)"],
  ["=ROUND(D19-D17,3)"],
  ["=ROUND(D19/D17,3)"],
  ["=ROUND(D16-D19,3)"],
  ["=ROUND(D18+D24,3)"],
];
sheet.getRange("E16:E25").values = [
  ["6400.000"],
  ["4300.000"],
  ["2911.200"],
  ["4060.000"],
  ["1148.800"],
  ["1.395"],
  ["-240.000"],
  ["0.944"],
  ["2340.000"],
  ["5251.200"],
];
setFormat(sheet.getRange("B15:E15"), {
  fill: "#1F4E78",
  font: { color: "#FFFFFF", bold: true },
  horizontalAlignment: "center",
  verticalAlignment: "center",
});
setFormat(sheet.getRange("B16:E25"), {
  borders: { preset: "outside", style: "thin", color: "#CBD5E1" },
  verticalAlignment: "center",
});
sheet.getRange("D16:D25").format.numberFormat = "0.000";
sheet.getRange("B20:E20").format.fill = "#DCFCE7";
sheet.getRange("B22:E22").format.fill = "#FEE2E2";
sheet.getRange("B24:E25").format.fill = "#FEF3C7";

sheet.getRange("O2:Z35").clear({ applyTo: "all" });
sheet.getRange("O2:Z2").values = [["Gantt planificado", ...weeks]];
sheet.getRange("O3:O10").values = activities.map((row) => [row[0]]);
sheet.getRange("O26:Z26").values = [["Situación real (semana 6)", ...weeks]];
sheet.getRange("O27:O34").values = activities.map((row) => [row[0]]);
setFormat(sheet.getRange("O2:Z2"), {
  fill: "#1F4E78",
  font: { color: "#FFFFFF", bold: true },
  horizontalAlignment: "center",
  verticalAlignment: "center",
});
setFormat(sheet.getRange("O26:Z26"), {
  fill: "#1F4E78",
  font: { color: "#FFFFFF", bold: true },
  horizontalAlignment: "center",
  verticalAlignment: "center",
});
setFormat(sheet.getRange("O3:Z10"), {
  borders: { preset: "outside", style: "thin", color: "#CBD5E1" },
  horizontalAlignment: "center",
  verticalAlignment: "center",
});
setFormat(sheet.getRange("O27:Z34"), {
  borders: { preset: "outside", style: "thin", color: "#CBD5E1" },
  horizontalAlignment: "center",
  verticalAlignment: "center",
});
sheet.getRange("O3:O10").format.fill = "#E5E7EB";
sheet.getRange("O27:O34").format.fill = "#E5E7EB";

activities.forEach((activity, idx) => {
  const plannedRow = idx + 3;
  const actualRow = idx + 27;
  const start = activity[3];
  const finish = activity[4];
  const completePct = activity[9];
  colorScheduled(plannedRow, start, finish, "#93C5FD");
  colorProgress(actualRow, start, finish, completePct);
});

// Estado a semana 6: línea vertical de referencia y trazo quebrado de avance real.
for (let row = 2; row <= 35; row++) {
  const range = sheet.getRange(cellName(ganttColumnForWeek(statusWeek), row));
  range.format.borders.getItem("EdgeLeft").style = "thick";
  range.format.borders.getItem("EdgeLeft").color = "#DC2626";
}
activities.forEach((activity, idx) => {
  const actualRow = idx + 27;
  const start = activity[3];
  const finish = activity[4];
  const completePct = activity[9];
  const progressPoint = start + (finish - start) * completePct;
  const markerWeek = Math.max(1, Math.min(11, Math.ceil(progressPoint)));
  markStatusLine(actualRow, markerWeek, "◆");
});
sheet.getRange("K26:L26").values = [["En semana", statusWeek]];
setFormat(sheet.getRange("K26:L26"), {
  fill: "#FEE2E2",
  font: { color: "#991B1B", bold: true },
  borders: { preset: "outside", style: "thin", color: "#DC2626" },
  horizontalAlignment: "center",
});

sheet.getRange("B28:E33").clear({ applyTo: "all" });
sheet.getRange("B28:E33").values = [
  ["Leyenda", null, null, null],
  ["Azul", "Trabajo planificado", null, null],
  ["Verde", "Trabajo realizado proporcionalmente", null, null],
  ["Rojo", "Semana 6 y línea quebrada de situación", null, null],
  ["VP", "Incluye avance planificado parcial de F y G a semana 6", null, null],
  ["ETC/EAC", "Se asume que las variaciones de coste no se repiten", null, null],
];
setFormat(sheet.getRange("B28:E28"), {
  fill: "#1F4E78",
  font: { color: "#FFFFFF", bold: true },
});
setFormat(sheet.getRange("B29:E33"), {
  borders: { preset: "outside", style: "thin", color: "#CBD5E1" },
  verticalAlignment: "center",
});
sheet.getRange("B29").format.fill = "#93C5FD";
sheet.getRange("B30").format.fill = "#22C55E";
sheet.getRange("B31").format.fill = "#FCA5A5";

unused.getRange("A1:AZ120").clear({ applyTo: "all" });
unused.getRange("A1:E4").values = [
  ["Hoja no usada en este ejercicio", null, null, null, null],
  ["Los cálculos requeridos están en la hoja 'Calculos Flujo de Caja'.", null, null, null, null],
  ["Se ha limpiado esta pestaña para evitar referencias rotas de la plantilla original.", null, null, null, null],
  [null, null, null, null, null],
];
setFormat(unused.getRange("A1:E3"), {
  fill: "#F8FAFC",
  font: { color: "#334155" },
  borders: { preset: "outside", style: "thin", color: "#CBD5E1" },
});

const keyCheck = await workbook.inspect({
  kind: "table",
  range: "Calculos Flujo de Caja!B15:E25",
  include: "values,formulas",
  tableMaxRows: 12,
  tableMaxCols: 4,
  summary: "EVM result check",
});
console.log(keyCheck.ndjson);

const errorCheck = await workbook.inspect({
  kind: "match",
  searchTerm: "#REF!|#DIV/0!|#VALUE!|#NAME\\?|#N/A",
  options: { useRegex: true, maxResults: 50 },
  summary: "formula errors",
});
console.log(errorCheck.ndjson);

await workbook.render({ sheetName: "Calculos Flujo de Caja", range: "B2:Z35", scale: 1 });
await workbook.render({ sheetName: "Caminocrítico", range: "A1:E4", scale: 1 });

const output = await SpreadsheetFile.exportXlsx(workbook);
await output.save(outputPath);
console.log(outputPath);
