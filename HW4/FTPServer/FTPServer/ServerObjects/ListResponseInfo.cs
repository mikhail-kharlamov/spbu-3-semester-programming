// <copyright file="ListResponseInfo.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace FTPServer.ServerObjects;

public record ListResponseInfo(int Size, ServerObjectInfo[] Objects);
