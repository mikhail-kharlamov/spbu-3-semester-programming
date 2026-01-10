// <copyright file="GetResponseInfo.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace FTPServer.ServerObjects;

public record GetResponseInfo(long Size, byte[] Data);
