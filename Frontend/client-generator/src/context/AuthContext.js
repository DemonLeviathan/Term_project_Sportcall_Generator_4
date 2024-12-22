// src/context/AuthContext.js
import React, { createContext, useState, useEffect } from 'react';
import jwtDecode from 'jwt-decode';
import PropTypes from 'prop-types';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [authData, setAuthData] = useState(() => {
    const token = localStorage.getItem('token');
    return token ? token : null;
  });

  const [user, setUser] = useState(() => {
    if (authData) {
      try {
        const decoded = jwtDecode(authData);
        return decoded;
      } catch (error) {
        console.error('Ошибка декодирования токена:', error);
        return null;
      }
    }
    return null;
  });

  useEffect(() => {
    if (authData) {
      localStorage.setItem('token', authData);
      try {
        const decoded = jwtDecode(authData);
        setUser(decoded);
      } catch (error) {
        console.error('Ошибка декодирования токена:', error);
        setUser(null);
      }
    } else {
      localStorage.removeItem('token');
      setUser(null);
    }
  }, [authData]);

  const logout = () => {
    setAuthData(null);
  };

  return (
    <AuthContext.Provider value={{ authData, setAuthData, user, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

AuthProvider.propTypes = {
  children: PropTypes.node.isRequired,
};
