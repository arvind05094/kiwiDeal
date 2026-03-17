import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import { useQueryClient } from "@tanstack/react-query";
import { getAccessToken } from "@/shared/api/client";

const HUB_URL = `${import.meta.env.VITE_API_URL?.replace("/api/v1", "") ?? "http://localhost:5158"}/hubs/notifications`;

export function useNotificationsHub() {
  const queryClient = useQueryClient();
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${HUB_URL}?access_token=${getAccessToken()}`)
      .withAutomaticReconnect()
      .build();

    connection.on("NotificationReceived", () => {
      queryClient.invalidateQueries({ queryKey: ["notifications"] });
    });

    connection.start();
    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, [queryClient]);
}
