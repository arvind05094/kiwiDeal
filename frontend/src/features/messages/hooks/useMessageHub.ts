import { useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import type { MessageDto } from "@/shared/types/common";
import { getAccessToken } from "@/shared/api/client";

const HUB_URL = `${import.meta.env.VITE_API_URL?.replace("/api/v1", "") ?? "http://localhost:5158"}/hubs/messages`;

export function useMessageHub(
  conversationId: string,
  initialMessages: MessageDto[],
) {
  const [messages, setMessages] = useState<MessageDto[]>(initialMessages);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    setMessages(initialMessages);
  }, [initialMessages]);

  useEffect(() => {
    if (!conversationId) return;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${HUB_URL}?access_token=${getAccessToken()}`)
      .withAutomaticReconnect()
      .build();

    connection.on("MessageReceived", (msg: MessageDto) => {
      setMessages((prev) => [...prev, msg]);
    });

    connection.start().then(() => {
      connection.invoke("JoinConversation", conversationId);
    });

    connectionRef.current = connection;

    return () => {
      connection.invoke("LeaveConversation", conversationId).finally(() => {
        connection.stop();
      });
    };
  }, [conversationId]);

  return { messages, setMessages };
}
