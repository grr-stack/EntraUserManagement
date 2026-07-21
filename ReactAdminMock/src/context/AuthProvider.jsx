import { useState } from 'react';
import { AuthContext } from './auth-context.js';
import { loginUser, registerUser } from '../lib/mockApi.js';

const SESSION_KEY = 'react-admin-session';

function getSavedSession() {
  const savedSession = localStorage.getItem(SESSION_KEY);
  return savedSession ? JSON.parse(savedSession) : null;
}

export function AuthProvider({ children }) {
  const [currentUser, setCurrentUser] = useState(getSavedSession);

  const handleLogin = async (credentials) => {
    const session = await loginUser(credentials);
    localStorage.setItem(SESSION_KEY, JSON.stringify(session));
    setCurrentUser(session);
    return session;
  };

  const handleRegister = async (payload) => {
    const session = await registerUser(payload);
    localStorage.setItem(SESSION_KEY, JSON.stringify(session));
    setCurrentUser(session);
    return session;
  };

  const logout = () => {
    localStorage.removeItem(SESSION_KEY);
    setCurrentUser(null);
  };

  return (
    <AuthContext.Provider
      value={{
        currentUser,
        isAuthenticated: Boolean(currentUser),
        loading: false,
        login: handleLogin,
        register: handleRegister,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}
