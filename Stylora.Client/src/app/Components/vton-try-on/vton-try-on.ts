import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VTONApiService } from '../../Services/vton-api.service';
import { IVTONRequest } from '../../Models/Interfaces/IVTONRequest';
import { IVTONResponse } from '../../Models/Interfaces/IVTONResponse';

@Component({
  selector: 'app-vton-try-on',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vton-try-on.html',
  styleUrl: './vton-try-on.css'
})
export class VtonTryOnComponent {
  personBase64: string | null = null;
  garmentBase64: string | null = null;
  personPreview: string | null = null;
  garmentPreview: string | null = null;
  clothDescription: string = '';
  resultBase64: string | null = null;
  isLoading: boolean = false;
  error: string | null = null;

  constructor(private VTONApi: VTONApiService) { }

  onPersonFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.convertFileToBase64(file, (base64) => {
        this.personBase64 = base64;
        this.personPreview = base64;
      });
    }
  }

  onGarmentFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.convertFileToBase64(file, (base64) => {
        this.garmentBase64 = base64;
        this.garmentPreview = base64;
      });
    }
  }

  // Helper function to convert File object to Base64 string (data URI format)
  private convertFileToBase64(file: File, callback: (base64: string) => void): void {
    const reader = new FileReader();
    reader.onload = () => {
      callback(reader.result as string); // result is the data URI string
    };
    reader.onerror = () => {
      this.error = 'Failed to read file.';
    };
    reader.readAsDataURL(file);
  }

  async tryOn(): Promise<void> {
    if (!this.personBase64 || !this.garmentBase64) {
      this.error = 'Please upload both a person and a garment image.';
      return;
    }

    this.isLoading = true;
    this.error = null;
    this.resultBase64 = null;

    const request: IVTONRequest = {
      personBase64: this.personBase64,
      garmentBase64: this.garmentBase64,
      clothDescription: this.clothDescription
    };

    try {
      const response: IVTONResponse = await new Promise((resolve, reject) => {
        this.VTONApi.tryOn(request).subscribe({
          next: (result) => resolve(result),
          error: (error) => reject(error)
        });
      });
      this.resultBase64 = response?.resultBase64 ?? null;
    } catch (err: any) {
      console.error('API Error:', err);
      this.error = err.error?.Details || 'An unknown error occurred during try-on.';
    } finally {
      this.isLoading = false;
    }
  }
}