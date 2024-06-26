// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Text.Encodings.Web;
using System.Text.Json;

namespace EdFi.DataManagementService.Core.Response;

/// <summary>
/// Provides human-readable summary of the failure responses
/// </summary>
internal record FailureResponse(
    /// <summary>
    /// A human-readable explanation specific to this occurrence of the problem.
    /// </summary>
    string detail,
    /// <summary>
    /// A URI reference [RFC3986] that identifies the problem type. This specification encourages that, when
    /// dereferenced, it provide human-readable documentation for the problem type
    /// (e.g., using HTML [W3C.REC-html5-20141028]).  When this member is not present, its value is assumed to be
    /// "about:blank".
    /// </summary>
    string type,
    /// <summary>
    /// A short, human-readable summary of the problem type.It SHOULD NOT change from occurrence to occurrence
    /// of the problem, except for purposes of localization(e.g., using proactive content negotiation;
    /// see[RFC7231], Section 3.4).
    /// </summary>
    string title,
    /// <summary>
    /// The HTTP status code([RFC7231], Section 6) generated by the origin server for this occurrence of the problem.
    /// </summary>
    int status,
    /// <summary>
    /// Gets or sets a correlation id that is logged with the details of the exception.
    /// </summary>
    string? correlationId,
    /// <summary>
    /// Contains arrays of validation errors keyed by JSON Path (intended for use by a domain expert).
    /// </summary>
    Dictionary<string, string[]>? validationErrors,
    /// <summary>
    /// Contains an array of error messages encountered while processing the request (intended for use by a technical audience).
    /// </summary>
    string[]? errors
)
{
    private const string BaseTypePrefix = "urn:ed-fi:api";
    private const string BadRequestTypePrefix = $"{BaseTypePrefix}:bad-request";
    private const string DataValidationTypePrefix = $"{BadRequestTypePrefix}:data";
    private const string NotFoundTypePrefix = $"{BaseTypePrefix}:not-found";
    private const string IdentityConflictTypePrefix = $"{BaseTypePrefix}:identity-conflict";

    public static FailureResponse ForDataValidation(
        string Detail,
        Dictionary<string, string[]>? ValidationErrors,
        string[]? Errors
    ) =>
        new(
            detail: Detail,
            type: DataValidationTypePrefix,
            title: "Data Validation Failed",
            status: 400,
            correlationId: null,
            validationErrors: ValidationErrors,
            errors: Errors
        );

    public static FailureResponse ForBadRequest(
        string Detail,
        Dictionary<string, string[]>? ValidationErrors,
        string[]? Errors
    ) =>
        new(
            detail: Detail,
            type: BadRequestTypePrefix,
            title: "Bad Request",
            status: 400,
            correlationId: null,
            validationErrors: ValidationErrors,
            errors: Errors
        );

    public static FailureResponse ForNotFound(
        string Detail
    ) =>
        new(
            detail: Detail,
            type: NotFoundTypePrefix,
            title: "Not Found",
            status: 404,
            correlationId: null,
            validationErrors: null,
            errors: null
        );

    public static FailureResponse ForIdentityConflict(
        string[]? Errors
    ) =>
        new(
            detail: "The identifying value(s) of the item are the same as another item that already exists.",
            type: IdentityConflictTypePrefix,
            title: "Identifying Values Are Not Unique",
            status: 409,
            correlationId: null,
            validationErrors: null,
            errors: Errors
        );

    public static string GenerateFrontendErrorResponse(string errorDetail)
    {
        var validationErrors = new Dictionary<string, string[]>();

        var value = new List<string>
        {
            errorDetail
        };
        validationErrors.Add("$.", value.ToArray());

        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var response = ForDataValidation(
            "Data validation failed. See 'validationErrors' for details.",
            validationErrors,
            new List<string>().ToArray()
        );

        return JsonSerializer.Serialize(response, options);
    }
}
