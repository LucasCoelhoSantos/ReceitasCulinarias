export interface AuthResponse {
  token: string;
  user: {
    id: string;
    userName: string;
    email: string;
  };
} 