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
    public class GeminiIAService
    {
        private readonly HttpClient http;
        private readonly IConfiguration config;

        public GeminiIAService(HttpClient http, IConfiguration config)
        {
            this.http = http;
            this.config = config;
        }

        public async Task<SintesisIAResultadoDTO> GenerarSintesisComentariosAsync(
            List<CriterioDashboardDTO> criterios)
        {
            if (criterios == null || !criterios.Any(c => c.Comentarios.Any()))
            {
                return new SintesisIAResultadoDTO
                {
                    sintesis = "No hay comentarios suficientes para generar una síntesis."
                };
            }

            var apiKey = config["Gemini:ApiKey"];

            var entradaJson = JsonSerializer.Serialize(criterios);

            var prompt = $$"""
            Eres un asistente que resume comentarios de evaluación de proyectos.

            Recibirás un JSON con criterios de evaluación y comentarios asociados.
            Devuelve ÚNICAMENTE un JSON válido con esta estructura exacta:

            {
                "sintesis": "Texto breve de máximo 4 frases",
                "pros": ["punto positivo 1", "punto positivo 2"],
                "contras": ["aspecto de mejora 1", "aspecto de mejora 2"]
            }

            No inventes información.
            No añadas texto fuera del JSON.

            JSON de entrada:
            {{entradaJson}}
            """;

            var request = new
            {
                contents = new[]
                {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
                generationConfig = new
                {
                    response_mime_type = "application/json"
                }
            };
             
            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            var response = await http.PostAsJsonAsync(url, request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new SintesisIAResultadoDTO
                {
                    sintesis = "No se pudo generar la síntesis porque se ha superado el límite gratuito de Gemini. Inténtalo de nuevo más tarde."
                };
            }

            using var doc = JsonDocument.Parse(json);

            var textoJson = doc.RootElement
    .GetProperty("candidates")[0]
    .GetProperty("content")
    .GetProperty("parts")[0]
    .GetProperty("text")
    .GetString();

            Console.WriteLine("RESPUESTA IA:");
            Console.WriteLine(textoJson);

            if (string.IsNullOrWhiteSpace(textoJson))
            {
                return new SintesisIAResultadoDTO
                {
                    sintesis = "No se pudo generar una síntesis."
                };
            }

            try
            {
                return JsonSerializer.Deserialize<SintesisIAResultadoDTO>(
                    textoJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new SintesisIAResultadoDTO
                {
                    sintesis = "No se pudo interpretar la respuesta de la IA."
                };
            }
            catch
            {
                return new SintesisIAResultadoDTO
                {
                    sintesis = textoJson
                };
            }
        }
    }
}
