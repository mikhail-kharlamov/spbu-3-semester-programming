// <copyright file="ServerObjectInfo.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace FTPServer.ServerObjects;

public record ServerObjectInfo(SystemObjectType Type, string Path);
