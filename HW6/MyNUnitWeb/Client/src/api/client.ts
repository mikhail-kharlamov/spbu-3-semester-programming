import axios from 'axios';
import type { TestRun } from '../types';

const API_URL = 'http://localhost:5126/api/testrunner';

export const api = {
    runTests: async (files: File[]): Promise<TestRun> => {
        const formData = new FormData();
        files.forEach(file => formData.append('files', file));

        const response = await axios.post<TestRun>(`${API_URL}/run`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        });
        return response.data;
    },

    getHistory: async (): Promise<TestRun[]> => {
        const response = await axios.get<TestRun[]>(`${API_URL}/history`);
        return response.data;
    }
};
