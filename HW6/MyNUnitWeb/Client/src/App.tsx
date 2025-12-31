import { useState, useEffect } from 'react';
import { Container, CssBaseline, AppBar, Toolbar, Typography, Box, Tabs, Tab } from '@mui/material';
import { TestUploader } from './components/TestUploader';
import TestResults from "./components/TestResults"
import type { TestRun } from './types';
import { api } from './api/client';

function App() {
    const [currentResult, setCurrentResult] = useState<TestRun | null>(null);
    const [history, setHistory] = useState<TestRun[]>([]);
    const [tab, setTab] = useState(0);

    const loadHistory = async () => {
        try {
            const data = await api.getHistory();
            setHistory(data);
        } catch (e) {
            console.error(e);
        }
    };

    useEffect(() => {
        if (tab === 1) loadHistory();
    }, [tab]);

    return (
        <>
            <CssBaseline />
            <AppBar position="static">
                <Toolbar>
                    <Typography variant="h6">MyNUnit Web</Typography>
                </Toolbar>
            </AppBar>

            <Container maxWidth="md" sx={{ mt: 4 }}>
                <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 3 }}>
                    <Tab label="Запуск тестов" />
                    <Tab label="История" />
                </Tabs>

                {tab === 0 && (
                    <Box>
                        <TestUploader onTestComplete={(res) => setCurrentResult(res)} />
                        {currentResult && <TestResults runData={currentResult} />}
                    </Box>
                )}

                {tab === 1 && (
                    <Box>
                        {history.map(run => (
                            <TestResults key={run.id} runData={run} />
                        ))}
                        {history.length === 0 && <Typography>История пуста</Typography>}
                    </Box>
                )}
            </Container>
        </>
    );
}

export default App;
