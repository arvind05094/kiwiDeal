import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import { getAccessToken } from "@/shared/api/client";

const HUB_URL = (
  import.meta.env.VITE_API_URL ?? "http://localhost:5158/api/v1"
).replace("/api/v1", "");

export function useListingHub(listingId: string, onListingSold: () => void) {
  const onListingSoldRef = useRef(onListingSold);

  useEffect(() => {
    onListingSoldRef.current = onListingSold;
  }, [onListingSold]);

  useEffect(() => {
    const token = getAccessToken();
    const url = token
      ? `${HUB_URL}/hubs/listings?access_token=${token}`
      : `${HUB_URL}/hubs/listings`;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(url)
      .withAutomaticReconnect()
      .build();

    connection.on("ListingSold", () => {
      onListingSoldRef.current();
    });

    connection
      .start()
      .then(() => connection.invoke("JoinListing", listingId))
      .catch((err) => console.error("ListingHub connect error:", err));

    return () => {
      connection
        .invoke("LeaveListing", listingId)
        .catch(() => {})
        .finally(() => connection.stop());
    };
  }, [listingId]);
}
