import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CryptoService {
  private privateRsaKey: CryptoKey | null = null;
  private publicRsaKey: CryptoKey | null = null;
  private aesKey: CryptoKey | null = null;

  async generateKeyPair(): Promise<void> {
    const keyPair = await crypto.subtle.generateKey(
      {
        name: 'RSA-OAEP',
        modulusLength: 2048,
        publicExponent: new Uint8Array([0x01, 0x00, 0x01]),
        hash: 'SHA-256',
      },
      true,
      ['encrypt', 'decrypt']
    );
    this.privateRsaKey = keyPair.privateKey;
    this.publicRsaKey = keyPair.publicKey;
  }

  async decryptWithRsaPrivateKey(encryptedData: string): Promise<ArrayBuffer> {
    if (!this.privateRsaKey) {
      throw new Error('Private key not generated');
    }
    const decodedData = Uint8Array.from(atob(encryptedData), (char) =>
      char.charCodeAt(0)
    );

    return crypto.subtle.decrypt(
      {
        name: 'RSA-OAEP',
      },
      this.privateRsaKey,
      decodedData
    );
  }

  async encryptWithRsa(data: string, publicRsaKey: string): Promise<string> {
    const keyBuffer = Uint8Array.from(atob(publicRsaKey), (char) =>
      char.charCodeAt(0)
    );

    const rsaCryptoKey = await crypto.subtle.importKey(
      'spki',
      keyBuffer.buffer,
      {
        name: 'RSA-OAEP',
        hash: 'SHA-256',
      },
      true,
      ['encrypt']
    );

    const encryptedBuffer = await crypto.subtle.encrypt(
      {
        name: 'RSA-OAEP',
      },
      rsaCryptoKey,
      new TextEncoder().encode(data)
    );

    return btoa(String.fromCharCode(...new Uint8Array(encryptedBuffer)));
  }

  async decryptWithAes(
    iv: ArrayBuffer,
    encryptedData: string
  ): Promise<ArrayBuffer> {
    if (!this.aesKey) {
      throw new Error('Aes key not imported!');
    }

    const decodedData = Uint8Array.from(atob(encryptedData), (c) =>
      c.charCodeAt(0)
    );

    const decrypted = await crypto.subtle.decrypt(
      {
        name: 'AES-CBC',
        iv,
      },
      this.aesKey,
      decodedData
    );

    return decrypted;
  }

  async importAesKey(aesKey: ArrayBuffer): Promise<void> {
    this.aesKey = await crypto.subtle.importKey(
      'raw',
      aesKey,
      { name: 'AES-CBC' },
      false,
      ['decrypt']
    );
  }

  async exportRsaPublicKey(): Promise<string> {
    if (!this.publicRsaKey) {
      throw new Error('Public key not generated');
    }

    const exportedKey = await crypto.subtle.exportKey(
      'spki',
      this.publicRsaKey
    );

    return btoa(String.fromCharCode(...new Uint8Array(exportedKey)));
  }

  getRsaPrivateKey(): CryptoKey | null {
    return this.privateRsaKey;
  }
}
