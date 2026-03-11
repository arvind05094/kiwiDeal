import axios from "axios";
const BASE_URL = import.meta.env.VITE_API_URL ?? "http://localhost:5158/api/v1";
let accessToken: string | null = null;
export const setAccessToken = (token: string | null) => {
  accessToken = token;
};
export const getAccessToken = () => accessToken;
const client = axios.create({
  baseURL: BASE_URL,
});
client.interceptors.request.use((config) => {
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});
client.interceptors.response.use(
  (response) => response,
  async (error) => {
    const original = error.config;
    if (error.response?.status === 401 && !original._retry) {
      original._retry = true;
      try {
        const refreshToken = localStorage.getItem("refreshToken");
        if (!refreshToken) {
          setAccessToken(null);
          window.location.href = "/login";
          return Promise.reject(error);
        }
        const res = await axios.post(`${BASE_URL}/auth/refresh`, {
          refreshToken,
        });
        localStorage.setItem("refreshToken", res.data.refreshToken);
        const newToken = res.data.accessToken;
        setAccessToken(newToken);
        original.headers.Authorization = `Bearer ${newToken}`;
        return client(original);
      } catch {
        setAccessToken(null);
        localStorage.removeItem("refreshToken");
        window.location.href = "/login";
      }
    }
    return Promise.reject(error);
  },
);
export default client;
