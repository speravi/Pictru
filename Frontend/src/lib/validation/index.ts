import { z } from "zod"
import { TagNames } from "../tags";

export const RegisterValidation = z.object({
  username: z.string().min(2, { message: 'Too short' }).default(""),
  email: z.string().email().default(""),
  password: z.string().min(8, { message: 'Too short' }).default(""),
});

export const UploadValidation = z.object({
  title: z.string().min(1, { message: 'Title is required' }).default(""),
  details: z.string().min(1, { message: 'Title is required' }).default(""),
  image: z.string().nonempty({ message: "Image is required" }).default(""),
  tags: z.array(z.nativeEnum(TagNames)).default([]),
  // tags: z.array(z.nativeEnum(TagNames)).nonempty("At least one tag is required").default([]),
});

export const LoginValidation = z.object({
  email: z.string().email().default(""),
  password: z.string().min(8, { message: 'Too short' }).default(""),
});

