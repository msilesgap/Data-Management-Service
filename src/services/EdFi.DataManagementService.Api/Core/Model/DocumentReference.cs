// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
namespace EdFi.DataManagementService.Api.Core.Model;

/// <summary>
/// Information representing a reference to a document, extracted from the referring document
/// </summary>
public record DocumentReference(
    /// <summary>
    /// Base API resource information for the referenced document
    /// </summary>
    BaseResourceInfo ResourceInfo,
    /// <summary>
    /// The document identity representing this reference.
    /// </summary>
    DocumentIdentity DocumentIdentity
)
{
    /// <summary>
    /// Returns the ReferentialId that represents this DocumentReference
    /// </summary>
    public ReferentialId ToReferentialId()
    {
        return DocumentIdentity.ToReferentialId(ResourceInfo);
    }
}
