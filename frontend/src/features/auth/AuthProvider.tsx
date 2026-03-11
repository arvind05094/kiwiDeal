import { useEffect, useState } from "react";
import type { UserResponse } from "@/shared/types/common";
import type { LoginRequest } from "./types";
import { setAccessToken } from "@/shared/api/client";
import axios from "axios";
import { AuthContext } from "./AuthContext";

const BASE_URL = import.meta.env.VITE_API_URL ?? "http://localhost:5158/api/v1";

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<UserResponse | null>(null);
  const [accessToken, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    axios
      .post(`${BASE_URL}/auth/refresh`, {}, { withCredentials: true })
      .then((res) => {
        setToken(res.data.accessToken);
        setAccessToken(res.data.accessToken);
        setUser(res.data.user);
      })
      .catch(() => {})
      .finally(() => setIsLoading(false));
  }, []);

  const login = async (email: string, password: string) => {
    const res = await axios.post<{ accessToken: string; user: UserResponse }>(
      `${BASE_URL}/auth/login`,
      { email, password } satisfies LoginRequest,
      { withCredentials: true },
    );
    setToken(res.data.accessToken);
    setAccessToken(res.data.accessToken);
    setUser(res.data.user);
  };

  const logout = async () => {
    try {
      await axios.post(
        `${BASE_URL}/auth/logout`,
        {},
        { withCredentials: true },
      );
    } catch {
      // intentional
    }
    setToken(null);
    setAccessToken(null);
    setUser(null);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        accessToken,
        isAuthenticated: !!accessToken,
        login,
        logout,
        isLoading,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}
