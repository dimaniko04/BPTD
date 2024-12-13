import { UserDto } from "../User/UserDto";

export interface AuthResponseDto {
    user: UserDto;
    token: string;
  }