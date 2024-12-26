import React, { useState, useEffect } from 'react';
import {
  Typography,
  Container,
  Box,
  TextField,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Alert,
  Pagination,
} from '@mui/material';
import api from '../../services/api';

const AdminDashboard = () => {
  const [users, setUsers] = useState([]);
  const [activities, setActivities] = useState([]);
  const [newActivity, setNewActivity] = useState({ activity_name: '', activity_type: '' });
  const [searchTerm, setSearchTerm] = useState('');
  const [userPage, setUserPage] = useState(1);
  const [activityPage, setActivityPage] = useState(1);
  const [userTotalPages, setUserTotalPages] = useState(1);
  const [activityTotalPages, setActivityTotalPages] = useState(1);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchUsers();
    fetchActivities();
  }, [userPage, activityPage]);

  const fetchUsers = async () => {
    try {
      const response = await api.get(`/user/users`, {
        params: { searchTerm, page: userPage, pageSize: 5 },
      });
      setUsers(response.data.users || []);
      setUserTotalPages(response.data.totalPages || 1);
    } catch (err) {
      console.error('Ошибка загрузки пользователей:', err);
      setError('Не удалось загрузить пользователей.');
    }
  };

  const fetchActivities = async () => {
    try {
      const response = await api.get(`/activity/activities`, {
        params: { page: activityPage, pageSize: 5 },
      });
      setActivities(response.data.activities || []);
      setActivityTotalPages(response.data.totalPages || 1);
    } catch (err) {
      console.error('Ошибка загрузки активностей:', err);
      setError('Не удалось загрузить активности.');
    }
  };

  const handleAddActivity = async () => {
    try {
      await api.post('/activity', newActivity);
      setNewActivity({ activity_name: '', activity_type: '' });
      fetchActivities();
    } catch (err) {
      console.error('Ошибка добавления активности:', err);
      setError('Не удалось добавить активность.');
    }
  };

  return (
    <Container>
      <Typography variant="h4" gutterBottom>
        Admin Panel
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      {/* Добавление активности */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h6">Добавить новую активность</Typography>
        <TextField
          label="Название активности"
          value={newActivity.activity_name}
          onChange={(e) => setNewActivity({ ...newActivity, activity_name: e.target.value })}
          fullWidth
          margin="normal"
        />
        <TextField
          label="Тип активности"
          value={newActivity.activity_type}
          onChange={(e) => setNewActivity({ ...newActivity, activity_type: e.target.value })}
          fullWidth
          margin="normal"
        />
        <Button variant="contained" onClick={handleAddActivity} sx={{ mt: 2 }}>
          Добавить
        </Button>
      </Box>

      {/* Список пользователей */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h6">Список пользователей</Typography>
        <TextField
          label="Поиск пользователя"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          fullWidth
          margin="normal"
        />
        <Button variant="contained" onClick={fetchUsers} sx={{ mt: 2 }}>
          Искать
        </Button>
        <TableContainer component={Paper} sx={{ mt: 2 }}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Имя пользователя</TableCell>
                <TableCell>Действия</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {users.map((user) => (
                <TableRow key={user.id}>
                  <TableCell>{user.username}</TableCell>
                  <TableCell>
                    <Button variant="outlined" onClick={() => alert(`Просмотр статистики ${user.username}`)}>
                      Статистика
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
        <Pagination
          count={userTotalPages}
          page={userPage}
          onChange={(e, value) => setUserPage(value)}
          sx={{ mt: 2 }}
        />
      </Box>

      {/* Список активностей */}
      <Box>
        <Typography variant="h6">Список активностей</Typography>
        <TableContainer component={Paper} sx={{ mt: 2 }}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Название активности</TableCell>
                <TableCell>Тип активности</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {activities.map((activity) => (
                <TableRow key={activity.activity_id}>
                  <TableCell>{activity.activity_name}</TableCell>
                  <TableCell>{activity.activity_type}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
        <Pagination
          count={activityTotalPages}
          page={activityPage}
          onChange={(e, value) => setActivityPage(value)}
          sx={{ mt: 2 }}
        />
      </Box>
    </Container>
  );
};

export default AdminDashboard;
