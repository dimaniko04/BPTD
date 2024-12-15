import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Router } from "@angular/router";
import { Observable } from "rxjs";
import { LoginDto } from "../models/Auth/LoginDto";
import { RegisterDto } from "../models/Auth/RegisterDto";
import { AuthResponseDto } from "../models/Auth/AuthResponseDto";

@Injectable({ providedIn: "root" })
export class AuthService {
  private apiUrl = "http://localhost:5200/auth";

  constructor(private http: HttpClient, private router: Router) {}

  register(registerDto: RegisterDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/register`, registerDto);
  }

  login(loginDto: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, loginDto);
  }

  saveToken(token: string): void {
    localStorage.setItem("authToken", token);
  }

  getToken(): string | null {
    return localStorage.getItem("authToken");
  }

  logout(): void {
    localStorage.removeItem("authToken");
    this.router.navigate(["/login"]);
  }

  isTokenExpired(token: string | null): boolean {
    if (!token) return true;

    const expiry = (JSON.parse(atob(token.split('.')[1]))).exp;
    return (Math.floor((new Date).getTime() / 1000)) >= expiry;
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (token && !this.isTokenExpired(token)) {
      return true;
    }

    this.logout();
    return false;
  }

  getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }
}

