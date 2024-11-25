import { Component, OnInit } from '@angular/core';
import { JSEncrypt } from 'jsencrypt';
import { EncryptedFileResponse, FileService } from '../services/file.service';
import { ToastrService } from 'ngx-toastr';
import { mergeMap } from 'rxjs';

@Component({
  selector: 'app-file-request',
  templateUrl: './file-request.component.html',
  styleUrls: ['./file-request.component.css'],
})
export class FileRequestComponent implements OnInit {
  fileName: string = '';
  private clientPublicKey: string = '';
  private clientPrivateKey: string = '';
  public serverPublicKey: string = '';

  constructor(
    private readonly fileService: FileService,
    private readonly toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.generateClientKeys();
    this.fetchServerPublicKey();
    this.registerClientPublicKey();

    this.decryptFile = this.decryptFile.bind(this);
  }

  private generateClientKeys() {
    const rsa = new JSEncrypt({ default_key_size: '2048' });
    this.clientPrivateKey = rsa.getPrivateKeyB64();
    this.clientPublicKey = rsa.getPublicKeyB64();
  }

  private fetchServerPublicKey() {
    this.fileService.getServerPublicKey().subscribe({
      next: (publicKey) => {
        this.serverPublicKey = publicKey;
        this.toastr.success('Публічний ключ сервера отримано.', 'Успіх');
      },
      error: (err) => {
        console.error('Помилка отримання ключа сервера:', err);
        this.toastr.error(
          'Не вдалося отримати публічний ключ сервера.',
          'Помилка'
        );
      },
    });
  }

  private registerClientPublicKey() {
    this.fileService.sendPublicKey(this.clientPublicKey).subscribe({
      next: () => {
        this.toastr.success(
          'Публічний ключ клієнта успішно зареєстровано.',
          'Успіх'
        );
      },
      error: (err) => {
        console.error('Помилка реєстрації ключа:', err);
        this.toastr.error(
          'Не вдалося зареєструвати публічний ключ.',
          'Помилка'
        );
      },
    });
  }

  requestFile() {
    if (!this.fileName) {
      this.toastr.warning('Будь ласка, введіть назву файлу.', 'Попередження');
      return;
    }

    const encryptor = new JSEncrypt();
    encryptor.setPublicKey(this.serverPublicKey);
    const encryptedFileName = encryptor.encrypt(this.fileName);

    if (!encryptedFileName) {
      this.toastr.error('Помилка шифрування імені файлу.', 'Помилка');
      return;
    }

    this.fileService
      .requestFile(encryptedFileName)
      .pipe(mergeMap(this.decryptFile))
      .subscribe({
        next: (response) => {
          if (!response) {
            this.toastr.error('Помилка розшифрування файлу.', 'Помилка');
          } else {
            this.downloadFile(response);
          }
        },
        error: (err) => {
          console.error('Помилка запиту файлу:', err);
          this.toastr.error('Не вдалося отримати файл.', 'Помилка');
        },
      });
  }

  private async decryptFile(response: EncryptedFileResponse) {
    const [key, iv] = this.decryptAesKeyAndIv(response.aesKey, response.iv);

    if (!key || !iv) {
      return null;
    }

    const textEncoder = new TextEncoder();
    const decryptedContent = await this.decryptAesContent(
      this.base64ToArrayBuffer(response.encryptedContent),
      textEncoder.encode(key),
      textEncoder.encode(iv)
    );

    return decryptedContent;
  }

  private decryptAesKeyAndIv(encryptedKey: string, encryptedIv: string) {
    const decryptor = new JSEncrypt();
    decryptor.setPrivateKey(this.clientPrivateKey);

    const key = decryptor.decrypt(encryptedKey);
    const iv = decryptor.decrypt(encryptedIv);

    return [key, iv];
  }

  private base64ToArrayBuffer(base64: string) {
    const binaryString = atob(base64);
    const bytes = new Uint8Array(binaryString.length);
    for (let i = 0; i < binaryString.length; i++) {
      bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes;
  }

  private async decryptAesContent(
    encryptedContent: Uint8Array,
    aesKey: Uint8Array,
    iv: Uint8Array
  ): Promise<ArrayBuffer> {
    const key = await crypto.subtle.importKey(
      'raw',
      aesKey,
      { name: 'AES-CBC' },
      false,
      ['decrypt']
    );

    return crypto.subtle.decrypt(
      {
        name: 'AES-CBC',
        iv,
      },
      key,
      encryptedContent
    );
  }

  private downloadFile(fileData: ArrayBuffer) {
    const blob = new Blob([fileData], {
      type: 'application/octet-stream',
    });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = this.fileName;
    a.click();
    window.URL.revokeObjectURL(url);
    this.toastr.success('Файл успішно розшифровано та завантажено.', 'Успіх');
  }
}
