// <copyright file="SystemObjectType.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace FTPServer.ServerObjects;

/// <summary>
/// Defines the kind of filesystem object resolved from a path, used by the client/server
/// protocol to distinguish files, directories, and missing entries.
/// </summary>
public enum SystemObjectType
{
    /// <summary>
    /// The path does not resolve to an existing filesystem object (missing or invalid).
    /// </summary>
    NotExists,

    /// <summary>
    /// The path points to a regular file that can be read or transferred.
    /// </summary>
    File,

    /// <summary>
    /// The path points to a directory that can be listed.
    /// </summary>
    Directory,
}
