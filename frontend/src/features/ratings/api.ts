import client from "@/shared/api/client";
import type { RatingPromptDto, SubmitRatingRequest } from "./types";

export const getRatingPrompts = async (): Promise<RatingPromptDto[]> => {
  const res = await client.get<RatingPromptDto[]>("/users/rating-prompts");
  return res.data;
};

export const submitRating = async (
  ratedUserId: string,
  body: SubmitRatingRequest,
): Promise<void> => {
  await client.post(`/users/${ratedUserId}/rate`, body);
};

export const dismissRatingPrompt = async (promptId: string): Promise<void> => {
  await client.delete(`/users/rating-prompts/${promptId}`);
};
