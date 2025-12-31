// <copyright file="index.ts" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>
export interface TestResult {
    id?: number;
    className: string;
    methodName: string;
    status: string;
    durationMs: number;
    message?: string;
    stackTrace?: string;
}

export interface TestAssembly {
    id?: number;
    assemblyName: string;
    passedCount: number;
    failedCount: number;
    ignoredCount: number;
    testResults: TestResult[];
}

export interface TestRun {
    id?: number;
    startedAt: string;
    status: string;
    assemblies: TestAssembly[];
}
