import {
    Card,
    CardContent,
    Typography,
    List,
    ListItem,
    ListItemText,
    Divider,
    Chip,
    Box
} from '@mui/material';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import ErrorIcon from '@mui/icons-material/Error';
import WarningIcon from '@mui/icons-material/Warning';
import type { TestRun } from '../types';

export default function TestResults({ runData }: { runData: TestRun | null }) {
    if (!runData) {
        return null;
    }

    const date = new Date(runData.startedAt).toLocaleString();

    return (
        <div style={{ marginTop: '20px' }}>
            <Typography variant="h4" gutterBottom>
                Отчет от {date}
            </Typography>

            {runData.assemblies.map((assembly) => (
                <Card key={assembly.id || assembly.assemblyName} sx={{ mb: 3, border: '1px solid #eee' }}>
                    <CardContent>
                        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                            <Typography variant="h6" color="primary">
                                📂 {assembly.assemblyName}
                            </Typography>
                            <Box>
                                <Chip icon={<CheckCircleIcon />} label={assembly.passedCount} color="success" sx={{ mr: 1 }} />
                                <Chip icon={<ErrorIcon />} label={assembly.failedCount} color="error" sx={{ mr: 1 }} />
                                <Chip icon={<WarningIcon />} label={assembly.ignoredCount} color="default" />
                            </Box>
                        </Box>

                        <Divider />

                        <List dense>
                            {assembly.testResults.map((test, i) => {
                                const isFailed = test.status === 'Failed';
                                const color = isFailed ? 'red' : (test.status === 'Passed' ? 'green' : 'orange');

                                return (
                                    <ListItem key={i} alignItems="flex-start" sx={{ bgcolor: isFailed ? '#fff0f0' : 'transparent' }}>
                                        <ListItemText
                                            primary={
                                                <span style={{ fontWeight: 'bold', color }}>
                                                    [{test.status}] {test.className}.{test.methodName}
                                                </span>
                                            }
                                            secondary={
                                                <>
                                                    <Typography component="span" variant="body2" color="text.secondary">
                                                        ⏱ {test.durationMs} ms
                                                    </Typography>
                                                    {test.message && (
                                                        <div style={{ marginTop: '5px', color: '#d32f2f', fontFamily: 'monospace', fontSize: '0.85em' }}>
                                                            ❌ {test.message}
                                                            {test.stackTrace && <pre>{test.stackTrace}</pre>}
                                                        </div>
                                                    )}
                                                </>
                                            }
                                        />
                                    </ListItem>
                                );
                            })}
                        </List>
                    </CardContent>
                </Card>
            ))}
        </div>
    );
}
