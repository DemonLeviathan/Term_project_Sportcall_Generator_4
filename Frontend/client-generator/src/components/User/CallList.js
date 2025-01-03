import React, { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Button,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from '@mui/material';

import api from '../../services/api';

function CallList() {
  const [calls, setCalls] = useState([]); 
  const [currentPage, setCurrentPage] = useState(1); 
  const pageSize = 5; 
  const [totalPages, setTotalPages] = useState(1); 
  const [error, setError] = useState(''); 

  useEffect(() => {
    fetchCallsData();
  }, []);

  useEffect(() => {
    console.log('Состояние вызовов:', calls);
  }, [calls]);
  

  
  
  const fetchCallsData = async () => {
    try {
      const username = localStorage.getItem("currentUser");
      if (!username) throw new Error("Нет имени пользователя в localStorage.");
  
      const response = await api.get("/call/user-calls", { params: { username } });
      const callData = response.data.calls;
  
      //const callIdMap = await fetchCallIds();
  
      const callsWithId = callData.map((call) => ({
        ...call,
        //call_id: callIdMap[call.call_id] || null, 
      }));
  
      setCalls(callsWithId);
  
      setTotalPages(
        callsWithId.length > 0 ? Math.ceil(callsWithId.length / pageSize) : 1
      );
    } catch (error) {
      console.error("Ошибка при загрузке вызовов:", error);
      setError("Не удалось загрузить список вызовов.");
    }
  };
  

  const updateCallStatus = async (callId, newStatus) => {
  if (!callId) {
    console.error('CallId отсутствует.');
    setError('Некорректные данные вызова.');
    return;
  }
  try {
    console.log("callId", callId);
    await api.post('/call/update-status', {
      callid : callId,
      status: newStatus,
    });

    setCalls((prevCalls) =>
      prevCalls.map((call) =>
        call.call_id === callId ? { ...call, status: newStatus } : call
      )
    );
  } catch (error) {
    console.error('Ошибка при обновлении статуса:', error.message || error);
    setError('Не удалось обновить статус вызова.');
  }
};

  const startIndex = (currentPage - 1) * pageSize;
  const currentCalls = calls.slice(startIndex, startIndex + pageSize);

  return (
    <Box sx={{ mb: 4 }}>
      <Typography variant="h6">Список вызовов</Typography>

      {error && (
        <Typography variant="body1" color="error" sx={{ mt: 2 }}>
          {error}
        </Typography>
      )}

      <TableContainer component={Paper} sx={{ mt: 2 }}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Название</TableCell>
              <TableCell>Описание</TableCell>
              <TableCell>Дата</TableCell>
              <TableCell>Статус</TableCell>
              <TableCell align="center">Действия</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
  {currentCalls.map((call) => (
    <TableRow key={call.call_id}>
      <TableCell>{call.call_name}</TableCell>
      <TableCell>{call.description}</TableCell>
      <TableCell>{call.call_date || 'Не указано'}</TableCell>
      <TableCell>{call.status}</TableCell>
      <TableCell align="center">
        <Button
          variant="contained"
          size="small"
          sx={{ mr: 1 }}
          onClick={() => updateCallStatus(call.call_id, 'completed')}
        >
          Выполнено
        </Button>
        <Button
          variant="outlined"
          size="small"
          onClick={() => updateCallStatus(call.call_id, 'failed')}
        >
          Не выполнено
        </Button>
      </TableCell>
    </TableRow>
  ))}

  {currentCalls.length === 0 && (
    <TableRow>
      <TableCell colSpan={5} align="center">
        Нет доступных вызовов
      </TableCell>
    </TableRow>
  )}
</TableBody>

        </Table>
      </TableContainer>

      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
        <Button
          variant="contained"
          disabled={currentPage === 1}
          onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
          sx={{ mr: 1 }}
        >
          Назад
        </Button>
        <Typography variant="body1" sx={{ alignSelf: 'center' }}>
          Страница {currentPage} из {totalPages}
        </Typography>
        <Button
          variant="contained"
          disabled={currentPage === totalPages}
          onClick={() => setCurrentPage((prev) => Math.min(prev + 1, totalPages))}
          sx={{ ml: 1 }}
        >
          Вперёд
        </Button>
      </Box>
    </Box>
  );
}

export default CallList;
