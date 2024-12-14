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

  isLoggedIn(): boolean {
    const token = this.getToken();
    return !!token;
  }

  getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }
}
