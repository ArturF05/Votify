import { FileBlob, SpreadsheetFile } from "@oai/artifact-tool";

const inputPath = "C:/Users/hugop/Downloads/Plantilla_B (2).xlsx";
const input = await FileBlob.load(inputPath);
const workbook = await SpreadsheetFile.importXlsx(input);

const summary = await workbook.inspect({
  kind: "workbook",
  summary: "Workbook structure",
});
console.log(summary.ndjson);
console.log("worksheets object keys", Object.keys(workbook.worksheets ?? {}));
try {
  console.log("worksheets names", workbook.worksheets.items?.map((s) => ({ id: s.id, name: s.name })));
} catch (error) {
  console.log("worksheet item read failed", error.message);
}

const sheets = ["Calculos Flujo de Caja", "Caminocrítico"];
for (const sheetName of sheets) {
  try {
    const table = await workbook.inspect({
      kind: "table",
      range: `${sheetName}!A1:Z80`,
      include: "values,formulas",
      tableMaxRows: 80,
      tableMaxCols: 26,
      summary: `Used area ${sheetName}`,
    });
    console.log(`--- ${sheetName} ---`);
    console.log(table.ndjson);
  } catch {
    // Sheet name probe; ignore misses.
  }
}

const help = await workbook.help("range.clear");
console.log("--- help range ---");
console.log(help.ndjson?.slice(0, 4000) ?? help);
