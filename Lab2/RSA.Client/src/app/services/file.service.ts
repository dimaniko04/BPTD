import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FileService {
  private apiUrl = 'https://localhost:7006';

  constructor(private http: HttpClient) {}

  sendPublicKey(publicKey: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/key/send?clientPublicKey=${encodeURIComponent(publicKey)}`, {});
  }

  getServerPublicKey(): Observable<string> {
    return this.http.get(`${this.apiUrl}/key/get`, { responseType: 'text' });
  }

  requestFile(encryptedFileName: string): Observable<string> {
    return this.http.get(`${this.apiUrl}/files?fileName=${encodeURIComponent(encryptedFileName)}`, { responseType: 'text' });
  } 
}
