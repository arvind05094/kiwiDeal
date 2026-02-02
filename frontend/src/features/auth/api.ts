import client from "@/shared/api/client";
import type { UserResponse } from "@/shared/types/common";
import type { LoginRequest, RegisterRequest } from "./types";

export async function loginApi(data: LoginRequest) {
  const res = await client.post<{ accessToken: string; user: UserResponse }>(
    "/auth/login",
    data,
  );
  return res.data;
}

export async function registerApi(data: RegisterRequest) {
  const res = await client.post<{ accessToken: string; user: UserResponse }>(
    "/auth/register",
    data,
  );
  return res.data;
}
