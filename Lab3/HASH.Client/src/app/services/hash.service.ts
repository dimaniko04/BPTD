import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class HashService {
  private apiUrl = 'https://localhost:7028';

  constructor(private http: HttpClient) {}

  hashText(text: string, bitSize: number): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/hash/text`, { text, bitSize });
  }

  hashFile(file: File, bitSize: number): Observable<string> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('bitSize', bitSize.toString());

    return this.http.post<string>(`${this.apiUrl}/hash/file`, formData);
  }

  generateFileCollision(file: File, bitSize: number): Observable<Blob> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('bitSize', bitSize.toString());

    return this.http.post(`${this.apiUrl}/collision/file`, formData, {
      responseType: 'blob',
    });
  }

  verifyText(text: string, digest: string, bitSize: number): Observable<boolean> {
    return this.http.post<boolean>(`${this.apiUrl}/verify/text`, { text, digest, bitSize });
  }

  verifyFile(file: File, digest: string, bitSize: number): Observable<boolean> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('digest', digest);
    formData.append('bitSize', bitSize.toString());

    return this.http.post<boolean>(`${this.apiUrl}/verify/file`, formData);
  }
}
