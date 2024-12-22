// src/routes/PrivateRoute.js
import React, { useContext } from 'react';
import { Navigate } from 'react-router-dom';
import PropTypes from 'prop-types';
import { AuthContext } from '../context/AuthContext';

const PrivateRoute = ({ children, roles }) => {
  const { authData, user } = useContext(AuthContext);

  if (!authData) {
    // Если нет токена, перенаправляем на страницу входа
    return <Navigate to="/login" />;
  }

  if (roles && roles.length > 0 && !roles.includes(user?.role)) {
    // Если роль пользователя не соответствует, перенаправляем на главную или другую страницу
    return <Navigate to="/login" />;
  }

  return children;
};

PrivateRoute.propTypes = {
  children: PropTypes.node.isRequired,
  roles: PropTypes.array,
};

PrivateRoute.defaultProps = {
  roles: [],
};

export default PrivateRoute;
