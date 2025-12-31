import React, { useState } from 'react';
import { Button, Box, Typography, Paper, LinearProgress, Alert } from '@mui/material';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import { api } from '../api/client';
import type { TestRun } from '../types';

interface Props {
    onTestComplete: (result: TestRun) => void;
}

export const TestUploader: React.FC<Props> = ({ onTestComplete }) => {
    const [files, setFiles] = useState<File[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files) {
            setFiles(Array.from(e.target.files));
            setError(null);
        }
    };

    const handleRun = async () => {
        if (files.length === 0) return;
        setLoading(true);
        setError(null);

        try {
            const result = await api.runTests(files);
            onTestComplete(result);
        } catch (err: any) {
            setError("Ошибка: " + (err.response?.data || err.message));
        } finally {
            setLoading(false);
        }
    };

    return (
        <Paper sx={{ p: 3, mb: 3 }}>
            <Typography variant="h6" gutterBottom>Новый запуск</Typography>
            <Box sx={{ display: 'flex', gap: 2, alignItems: 'center', mb: 2 }}>
                <Button component="label" variant="outlined" startIcon={<CloudUploadIcon />}>
                    Выбрать DLL
                    <input type="file" hidden multiple accept=".dll" onChange={handleFileChange} />
                </Button>
                <Typography variant="body2">
                    {files.length > 0 ? `Файлов: ${files.length}` : 'Файлы не выбраны'}
                </Typography>
            </Box>
            {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
            <Button variant="contained" onClick={handleRun} disabled={files.length === 0 || loading} fullWidth>
                {loading ? 'Тестирование...' : 'Начать тестирование'}
            </Button>
            {loading && <LinearProgress sx={{ mt: 2 }} />}
        </Paper>
    );
};
