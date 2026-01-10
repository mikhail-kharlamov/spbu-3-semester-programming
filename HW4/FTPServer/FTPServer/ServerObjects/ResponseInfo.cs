// <copyright file="ResponseInfo.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace FTPServer.ServerObjects;

public record ResponseInfo(byte[] Buffer, int Length);
