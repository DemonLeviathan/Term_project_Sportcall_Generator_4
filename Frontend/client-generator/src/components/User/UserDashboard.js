import React, { useState, useEffect, useContext } from 'react';
import { AuthContext } from '../../context/AuthContext'; 
import {
  Typography,
  Container,
  Box,
  TextField,
  Button,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Alert,
  MenuItem,
  Select,
  FormControl,
  InputLabel,
} from '@mui/material';
import api from '../../services/api'; // Axios instance with interceptors

const UserDashboard = () => {
  const { user } = useContext(AuthContext); 
  const [users, setUsers] = useState([]);
  const [friends, setFriends] = useState([]);
  const [notifications, setNotifications] = useState([]);
  const [userData, setUserData] = useState([]); 
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(0);
  const [activities, setActivities] = useState([]);
  const [selectedType, setSelectedType] = useState('');
  const [activityTypes, setActivityTypes] = useState([]); 
  const [filteredActivities, setFilteredActivities] = useState([]); 
  const [newUserData, setNewUserData] = useState({
    activity_name: '',
    weight: '',
    height: '',
  }); 
  const [searchTerm, setSearchTerm] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  useEffect(() => {
    if (user) {
      fetchFriends();
      fetchNotifications();
      fetchActivityTypes();
    } else {
      console.error('Текущий пользователь не определён.');
      setError('Текущий пользователь не определён.');
    }
  }, [user]);

  useEffect(() => {
    fetchUserData();
  }, [currentPage]);
  
  useEffect(() => {
    console.log("Обновленные данные пользователя:", userData);
  }, [userData]);
  
  const fetchUsers = async () => {
    try {
      const currentUserId = localStorage.getItem('currentUser');
      const currentUserRole = localStorage.getItem('currentUserRole');

      if (!currentUserId || !currentUserRole) {
        setError('Текущий пользователь не определён.');
        return;
      }

      const response = await api.get('/user/users', { params: { searchTerm } });
      const filteredUsers = response.data.users.filter((user) => {
        console.log('Проверяем пользователя:', user);
        return user.username !== currentUserId && user.user_role !== 'Admin';
      });

      console.log('Отфильтрованный список пользователей:', filteredUsers);
      setUsers(filteredUsers);
    } catch (err) {
      console.error('Ошибка загрузки пользователей:', err);
      setError('Не удалось загрузить пользователей.');
    }
  };

  const fetchFriends = async () => {
    try {
        const username = localStorage.getItem('currentUser');
        if (!username) throw new Error('Имя пользователя отсутствует.');

        const userResponse = await api.get('/user/get-id', { params: { username } });
        const userId = userResponse.data.user_id;

        const response = await api.get('/friendship/list', { params: { userId } });
        setFriends(response.data || []);
    } catch (err) {
        console.error('Ошибка загрузки друзей:', err.response?.data || err.message);
        setError('Не удалось загрузить список друзей.');
    }
};


const fetchNotifications = async () => {
  try {
      const username = localStorage.getItem('currentUser');
      if (!username) throw new Error('Имя пользователя отсутствует.');

      const userResponse = await api.get('/user/get-id', { params: { username } });
      const userId = userResponse.data.user_id;

      const response = await api.get('/friendship/notifications', { params: { userId } });
      const notifications = response.data || [];

      console.log('Полученные уведомления:', notifications);
      setNotifications(notifications);
  } catch (err) {
      console.error('Ошибка загрузки уведомлений:', err.response?.data || err.message);
      setError('Не удалось загрузить уведомления.');
  }
};



  const fetchUserData = async () => {
    try {
      const response = await api.get(`/userdata/all`, {
        params: { username: localStorage.getItem("currentUser"), page: currentPage, size: pageSize },
      });
      const { records, totalPages } = response.data;
  
      setUserData(records);
      setTotalPages(totalPages);
    } catch (err) {
      console.error("Ошибка загрузки данных пользователя:", err);
      setError("Не удалось загрузить данные пользователя.");
    }
  };  
  

  const fetchActivityTypes = async () => {
    try {
        const response = await api.get('/activity/types'); 
        setActivityTypes(response.data || []);
    } catch (err) {
        console.error('Ошибка загрузки типов активностей:', err);
        setError('Не удалось загрузить типы активностей.');
    }
};

const fetchActivitiesByType = async (type) => {
    try {
        if (!type) {
            setFilteredActivities([]); 
            return;
        }

        const response = await api.get('/activity/by-type', { params: { activityType: type } }); 
        setFilteredActivities(response.data || []);
    } catch (err) {
        console.error('Ошибка загрузки активностей по типу:', err);
        setError('Не удалось загрузить список активностей.');
    }
};


  const handleTypeChange = (type) => {
    setSelectedType(type);
    setFilteredActivities(activities.filter((activity) => activity.activity_type === type));
    setNewUserData({ ...newUserData, activity_name: '' }); 
  };

  const handleAddUserData = async () => {
    try {
      if (!newUserData.activity_name) {
        setError('Название активности обязательно.');
        return;
      }

      await api.post('/userData', {
        ...newUserData,
        user_id: user.sub,
      });
      setSuccess('Данные успешно добавлены!');
      setTimeout(() => setSuccess(''), 3000);
      setNewUserData({ activity_name: '', weight: '', height: '' });
    } catch (err) {
      console.error('Ошибка добавления данных пользователя:', err);
      setError('Не удалось добавить данные пользователя.');
    }
  };


  const handleAddFriend = async (id) => {
    try {
        const username = localStorage.getItem('currentUser'); 
        if (!username) throw new Error('Имя пользователя отсутствует в localStorage.');

        const userResponse = await api.get('/user/get-id', { params: { username } });
        const userId = userResponse.data.user_id;

        const payload = {
            user1_id: userId,
            user2_id: id,
        };

        console.log('Payload для добавления в друзья:', payload);

        const response = await api.post('/friendship/add', payload);
        console.log('Ответ сервера:', response.data);

        setSuccess('Запрос отправлен!');
        setTimeout(() => setSuccess(''), 3000);
        fetchNotifications();
    } catch (err) {
        console.error('Ошибка добавления в друзья:', err.response?.data || err.message);
        setError('Не удалось отправить запрос.');
    }
};


const handleRespondNotification = async (notificationId, recieverName, recieverId, senderId, senderName, accept) => {
  try {
      const username = localStorage.getItem('currentUser');
      if (!username) throw new Error('Имя пользователя отсутствует в localStorage.');

      const userResponse = await api.get('/user/get-id', { params: { username } });
      const userId = userResponse.data.user_id;

      console.log("Id текущег пользователя", userId);

      const payload = {
          friend_id: notificationId,
          user1_id: senderId,
          user2_id: recieverId,
          IsPending: !accept,
      };

      console.log('Payload для обработки уведомления:', payload);

      const response = await api.post(
          `/friendship/respond`,
          payload,
          { params: { accept } }
      );

      console.log('Ответ сервера:', response.data);
      setSuccess('Запрос обработан успешно!');

      setNotifications((prevNotifications) =>
          prevNotifications.filter((notification) => notification.friend_id !== notificationId)
      );

      fetchFriends(); 
  } catch (err) {
      console.error('Ошибка обработки уведомления:', err.response?.data || err.message);
      setError('Не удалось обработать запрос.');
  }
};



  if (!user) {
    return (
      <Typography variant="h6">
        Текущий пользователь не определён. Пожалуйста, войдите в систему.
      </Typography>
    );
  }

  return (
    <Container>
      <Typography variant="h4" gutterBottom>
        Страница Пользователя
      </Typography>

      {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {/* Добавление пользовательских данных */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h6">Добавить данные пользователя</Typography>
        
        {/* Выпадающий список типов активностей */}
        <TextField
            select
            label="Тип активности"
            value={newUserData.activity_type || ''}
            onChange={(e) => {
                const selectedType = e.target.value;
                setNewUserData({ ...newUserData, activity_type: selectedType });
                fetchActivitiesByType(selectedType); 
            }}
            fullWidth
            margin="normal"
            SelectProps={{
                native: true,
                displayEmpty: true,
            }}
            sx={{
              '& .MuiInputLabel-root': { display: newUserData.activity_type ? 'block' : 'none' },
          }}
        >
            <option value="">Выберите тип активности</option>
            {activityTypes.map((type) => (
                <option key={type} value={type}>
                    {type}
                </option>
            ))}
        </TextField>

        {/* Выпадающий список названий активностей */}
        <TextField
            select
            label="Название активности"
            value={newUserData.activity_name || ''}
            onChange={(e) => setNewUserData({ ...newUserData, activity_name: e.target.value })}
            fullWidth
            margin="normal"
            SelectProps={{
                native: true,
                displayEmpty: true,
            }}
            sx={{
              '& .MuiInputLabel-root': { display: newUserData.activity_type ? 'block' : 'none' },
          }}
        >
            <option value="">Выберите активность</option>
            {filteredActivities.map((activity) => (
                <option key={activity.activity_name} value={activity.activity_name}>
                    {activity.activity_name}
                </option>
            ))}
        </TextField>

        {/* Поля для ввода веса и роста */}
        <TextField
            label="Вес (кг)"
            value={newUserData.weight}
            onChange={(e) => setNewUserData({ ...newUserData, weight: e.target.value })}
            type="number"
            fullWidth
            margin="normal"
        />
        <TextField
            label="Рост (см)"
            value={newUserData.height}
            onChange={(e) => setNewUserData({ ...newUserData, height: e.target.value })}
            type="number"
            fullWidth
            margin="normal"
        />
        <Button variant="contained" onClick={handleAddUserData} sx={{ mt: 2 }}>
            Добавить
        </Button>
    </Box>

    {/* Список данных пользователя с пагинацией */}
    <Box sx={{ mb: 4 }}>
    <Typography variant="h6">Ваши данные</Typography>
  <TableContainer component={Paper}>
    <Table>
      <TableHead>
        <TableRow>
          <TableCell>Дата</TableCell>
          <TableCell>Тип активности</TableCell>
          <TableCell>Название активности</TableCell>
          <TableCell>Вес (кг)</TableCell>
          <TableCell>Рост (см)</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {userData.map((data) => (
          <TableRow key={data.data_id}>
            <TableCell>{new Date(data.date_info).toLocaleDateString()}</TableCell>
            <TableCell>{data.activityType || "Нет данных"}</TableCell>
            <TableCell>{data.activityName || "Нет данных"}</TableCell>
            <TableCell>{data.weight}</TableCell>
            <TableCell>{data.height}</TableCell>
          </TableRow>
        ))}
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

      {/* функционал: поиск, друзья, уведомления */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h6">Найти пользователя</Typography>
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
                <TableCell>Действие</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {users.map((user) => (
                <TableRow key={user.user_id}>
                  <TableCell>{user.username}</TableCell>
                  <TableCell>
                    <Button
                      variant="outlined"
                      onClick={() => handleAddFriend(user.user_id)}
                    >
                      Добавить в друзья
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Box>

      {/* Список друзей */}
      <Box sx={{ mb: 4 }}>
          <Typography variant="h6">Ваши друзья</Typography>
          {friends.length === 0 ? (
              <Typography variant="body1" sx={{ mt: 2 }}>
                  Нет друзей
              </Typography>
          ) : (
              <TableContainer component={Paper} sx={{ mt: 2 }}>
                  <Table>
                      <TableHead>
                          <TableRow>
                              <TableCell>Имя друга</TableCell>
                          </TableRow>
                      </TableHead>
                      <TableBody>
                      {friends.length > 0 ? (
                          friends.map((friend) => (
                              <div key={friend.friendId}>
                                  <p>    {friend.friendName || "Имя не указано"}</p>
                              </div>
                          ))
                      ) : (
                          <p>Друзей нет.</p>
                      )}

                      </TableBody>
                  </Table>
              </TableContainer>
          )}
      </Box>



      {/* Уведомления */}
      <Box sx={{ mb: 4 }}>
          <Typography variant="h6">Уведомления</Typography>
          {notifications.length === 0 ? (
              <Typography variant="body1" sx={{ mt: 2 }}>
                  Нет уведомлений
              </Typography>
          ) : (
              <TableContainer component={Paper} sx={{ mt: 2 }}>
                  <Table>
                      <TableHead>
                          <TableRow>
                              <TableCell>Имя пользователя</TableCell>
                              <TableCell>Действие</TableCell>
                          </TableRow>
                      </TableHead>
                      <TableBody>
                        {notifications.length > 0 ? (
                            notifications.map((notification) => (
                                <TableRow key={notification.friend_id}>
                                    <TableCell>{notification.senderName || 'Имя отсутствует'}</TableCell>
                                    <TableCell>
                                        <Button
                                            variant="contained"
                                            onClick={() => handleRespondNotification(notification.friend_id, notification.recieverName, notification.recieverId, notification.senderId, notification.senderName, true)}
                                            sx={{ mr: 1 }}
                                        >
                                            Принять
                                        </Button>
                                        <Button
                                            variant="outlined"
                                            onClick={() => handleRespondNotification(notification.friend_id, notification.recieverName, notification.recieverId, notification.senderId, notification.senderName, false)}
                                        >
                                            Отклонить
                                        </Button>
                                    </TableCell>
                                </TableRow>
                            ))
                        ) : (
                            <TableRow>
                                <TableCell colSpan={2}>Нет уведомлений</TableCell>
                            </TableRow>
                        )}
                    </TableBody>

                  </Table>
              </TableContainer>
          )}
      </Box>

    </Container>
  );
};

export default UserDashboard;
