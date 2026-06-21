using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CatalogAPI.Extensions.SwaggerDocumentation
{
    public class CustomOperationDescriptions : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context?.ApiDescription?.HttpMethod is null || context.ApiDescription.RelativePath is null)
                return;

            var path = context.ApiDescription.RelativePath.ToLowerInvariant();

            var routeHandlers = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
            {
                { "games",               () => HandleGamesOperations(operation, context) }

            };

            foreach (var kv in routeHandlers
                     .OrderByDescending(k => k.Key.Length))
            {
                if (path.Contains(kv.Key))
                {
                    kv.Value.Invoke();
                    return;
                }
            }
        }

        private void HandleGamesOperations(OpenApiOperation operation, OperationFilterContext context)
        {
            var method = context.ApiDescription.HttpMethod;
            var path = context.ApiDescription.RelativePath?.ToLower() ?? string.Empty;

            if (method == "POST")
            {
                if (path.Contains("purchase"))
                {
                    operation.Summary = "Start game purchase flow.";
                    operation.Description = "This endpoint starts the game purchase flow for an authenticated user. In the event-driven flow, it will publish an OrderPlacedEvent containing UserId, GameId and Price.";
                    AddResponses(operation, "200", "The game purchase flow was successfully started.");
                    AddResponses(operation, "400", "Invalid purchase request.");
                    AddResponses(operation, "401", "User is not authenticated.");
                    AddResponses(operation, "404", "Game not found or inactive.");
                }
                else
                {
                    operation.Summary = "Create a new Game.";
                    operation.Description = "This endpoint allows an administrator to create a new Game by providing the necessary details.";
                    AddResponses(operation, "200", "The Game was successfully created.");
                    AddResponses(operation, "401", "User is not authenticated.");
                    AddResponses(operation, "403", "User does not have permission to create games.");
                }
            }
            else if (method == "PUT")
            {
                operation.Summary = "Update an existing Game.";
                operation.Description = "This endpoint allows an administrator to update an existing Game by providing the necessary details.";
                AddResponses(operation, "200", "The Game was successfully updated.");
                AddResponses(operation, "401", "User is not authenticated.");
                AddResponses(operation, "403", "User does not have permission to update games.");
                AddResponses(operation, "404", "Game not found. Please verify the ID.");
            }
            else if (method == "DELETE")
            {
                operation.Summary = "Delete an existing Game.";
                operation.Description = "This endpoint allows an administrator to delete an existing Game by providing the ID.";
                AddResponses(operation, "200", "The Game was successfully deleted.");
                AddResponses(operation, "401", "User is not authenticated.");
                AddResponses(operation, "403", "User does not have permission to delete games.");
                AddResponses(operation, "404", "Game not found. Please verify the ID.");
            }
            else if (method == "GET")
            {
                if (path.Contains("{id}"))
                {
                    operation.Summary = "Retrieve game by id.";
                    operation.Description = "This endpoint is responsible for retrieving an active game by id.";
                    AddResponses(operation, "200", "Game retrieved successfully.");
                    AddResponses(operation, "401", "User is not authenticated.");
                    AddResponses(operation, "404", "Game not found. Please verify the ID.");
                }
                else if (path.Contains("all"))
                {
                    operation.Summary = "Retrieve all games.";
                    operation.Description = "This endpoint is responsible for retrieving all games.";
                    AddResponses(operation, "200", "All games retrieved successfully.");
                    AddResponses(operation, "401", "User is not authenticated.");
                }
            }
        }

        private void AddResponses(OpenApiOperation operation, string statusCode, string description)
        {
            if (!operation.Responses.ContainsKey(statusCode))
            {
                operation.Responses.Add(statusCode, new OpenApiResponse { Description = description });
            }
        }
    }
}