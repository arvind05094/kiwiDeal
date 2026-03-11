import { useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import type { MessageDto } from "@/shared/types/common";
import { getAccessToken } from "@/shared/api/client";

const HUB_URL = `${import.meta.env.VITE_API_URL?.replace("/api/v1", "") ?? "http://localhost:5158"}/hubs/messages`;

export function useMessageHub(
  conversationId: string,
  initialMessages: MessageDto[],
) {
  const [liveMessages, setLiveMessages] = useState<MessageDto[]>([]);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    if (!conversationId) return;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${HUB_URL}?access_token=${getAccessToken()}`)
      .withAutomaticReconnect()
      .build();

    connection.on("MessageReceived", (msg: MessageDto) => {
      const normalized: MessageDto = {
        id: msg.id,
        conversationId: msg.conversationId,
        senderId: msg.senderId,
        senderName: msg.senderName,
        content: msg.content,
        createdAt: msg.createdAt,
      };
      setLiveMessages((prev) => {
        if (prev.some((m) => m.id === normalized.id)) return prev;
        return [...prev, normalized];
      });
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

  const allIds = new Set(initialMessages.map((m) => m.id));
  const messages = [
    ...initialMessages,
    ...liveMessages.filter(
      (m) => m.conversationId === conversationId && !allIds.has(m.id),
    ),
  ];

  const addMessage = (msg: MessageDto) => {
    setLiveMessages((prev) => {
      if (prev.some((m) => m.id === msg.id)) return prev;
      return [...prev, msg];
    });
  };

  return { messages, addMessage };
}
