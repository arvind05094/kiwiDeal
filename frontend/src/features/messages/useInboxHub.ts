import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import { useQueryClient } from "@tanstack/react-query";
import { getAccessToken } from "@/shared/api/client";

const HUB_URL = `${import.meta.env.VITE_API_URL?.replace("/api/v1", "") ?? "http://localhost:5158"}/hubs/messages`;

export function useInboxHub() {
  const queryClient = useQueryClient();
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${HUB_URL}?access_token=${getAccessToken()}`)
      .withAutomaticReconnect()
      .build();

    connection.on("ConversationUpdated", () => {
      queryClient.invalidateQueries({ queryKey: ["conversations"] });
    });

    connection.start();
    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, [queryClient]);
}
