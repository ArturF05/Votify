using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Votify.Core.DTOs;

namespace Votify.Core.Services
{
    public class IAService
    {
        private readonly HttpClient http;

        public IAService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<SintesisIAResultadoDTO> GenerarSintesisComentariosAsync(List<CriterioDashboardDTO> criterios)
        {
            if (criterios == null || !criterios.Any(c => c.Comentarios.Any()))
            {
                return new SintesisIAResultadoDTO
                {
                    sintesis = "No hay comentarios suficientes para generar una síntesis."
                };
            }

            var entradaJson = JsonSerializer.Serialize(criterios);

            var prompt = $$"""
Eres un asistente que resume comentarios de evaluación de proyectos.

Recibirás un JSON con criterios de evaluación y comentarios asociados.
Devuelve ÚNICAMENTE un JSON válido con esta estructura exacta:

{
  "sintesis": "Texto natural, con tono profesional y cercano",
  "pros": ["punto positivo redactado de forma clara", "otro punto positivo"],
  "contras": ["aspecto de mejora redactado de forma constructiva", "otro aspecto de mejora"]
}

Genera una síntesis natural, clara y útil para un dashboard de resultados.
No debes mencionar el dashboard
El tono debe ser profesional pero cercano, sin ofender al equipo, en tercera persona como si estuvieras explicando el feedback al equipo del proyecto.
No seas seco ni telegráfico.
Usa frases completas y fluidas.
No inventes información. No rellenes. Habla solo de la información que obtienes a través de los comentarios.
No añadas texto fuera del JSON.

JSON de entrada:
{{entradaJson}}
""";

            var request = new 
            {
                model = "llama3.2",
                prompt = prompt,
                stream = false,
                format = "json"
            };

            var response = await http.PostAsJsonAsync(
                "http://localhost:11434/api/generate",
                request
            );

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new SintesisIAResultadoDTO
                {
                    sintesis = "No se pudo generar la síntesis porque Ollama no respondió correctamente."
                };
            }

            using var doc = JsonDocument.Parse(json);

            var texto = doc.RootElement
                .GetProperty("response")
                .GetString();

            if (string.IsNullOrWhiteSpace(texto))
            {
                return new SintesisIAResultadoDTO
                {
                    sintesis = "No se pudo generar una síntesis válida."
                };
            }

            try
            {
                return JsonSerializer.Deserialize<SintesisIAResultadoDTO>(
                    texto,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new SintesisIAResultadoDTO
                {
                    sintesis = texto
                };
            }
            catch
            {
                return new SintesisIAResultadoDTO
                {
                    sintesis = "No se pudo conectar con Ollama. Asegúrate de que está iniciado antes de generar la síntesis."
                };
            }
        }
    }
}
