import { AfterViewInit, Component, OnDestroy, ViewChild } from '@angular/core';
import { BarcodeFormat } from '@zxing/library';
import { Html5QrcodeSupportedFormats } from 'html5-qrcode';
import { Html5Qrcode } from 'html5-qrcode/esm/html5-qrcode';
import { BarcodeScannerLivestreamComponent, BarcodeScannerLivestreamModule } from "ngx-barcode-scanner";
@Component({
  selector: 'app-checkin-client',
  templateUrl: './checkin-client.component.html',
  styleUrls: ['./checkin-client.component.css']
})
export class CheckinClientComponent implements AfterViewInit, OnDestroy {
  html5QrCode!: Html5Qrcode;
  scannedResult: string | null = null;

  ngAfterViewInit(): void {
    this.html5QrCode = new Html5Qrcode('reader');

    const config = {
      fps: 5,
      qrbox: 250,
      supportedScanTypes: [Html5QrcodeSupportedFormats.CODE_128],
    };

    this.html5QrCode.start(
      { facingMode: 'environment' },
      config,
      (decodedText) => {
        this.scannedResult = decodedText;
        console.log('Scanned:', decodedText);
        this.html5QrCode.stop(); // stop after first scan
      },
      (errorMessage) => {
        // console.warn('Scan error:', errorMessage);
      }
    ).catch(err => console.error('Start failed:', err));
  }

  ngOnDestroy(): void {
    if (this.html5QrCode) {
      this.html5QrCode.stop().catch(() => {});
    }
  }
}


// implements AfterViewInit {
  
//   @ViewChild(BarcodeScannerLivestreamComponent)
//   barcodeScanner!: BarcodeScannerLivestreamComponent;

//   barcodeValue: string | null = null;

//   constructor() {}

//   ngAfterViewInit() {
//     if (this.barcodeScanner) {
//       this.barcodeScanner.start();
//     } else {
//       console.warn('BarcodeScannerLivestreamComponent not found');
//     }
//   }

//   onValueChanges(result: any) {
//     if (result && result.codeResult && result.codeResult.code) {
//       this.barcodeValue = result.codeResult.code;
//       console.log('Scanned barcode:', this.barcodeValue);
//     }
//   }

//   onStarted(started: boolean) {
//     console.log('Scanner started:', started);
//   }
// }



// implements AfterViewInit {

//    scannedResult: string = '';
//    start:any

//    allowedFormats = [
//   // BarcodeFormat.CODE_39,
//     BarcodeFormat.CODE_128,
//     // BarcodeFormat.EAN_13,
//     // BarcodeFormat.EAN_8,
//     // BarcodeFormat.PDF_417,
//     // BarcodeFormat.QR_CODE,
//     // BarcodeFormat.DATA_MATRIX,
//     // BarcodeFormat.AZTEC
// ];

//   @ViewChild(BarcodeScannerLivestreamComponent)
//   barcodeScanner: BarcodeScannerLivestreamComponent;

//   barcodeValue;

//   ngAfterViewInit() {
//     this.barcodeScanner.start();
//   }

//   onValueChanges(result) {
//     this.barcodeValue = result.codeResult.code;
//   }

//   onStarted(started) {
//     console.log(started);
//   }

//   handleScanSuccess(result: string): void {
//     this.scannedResult = result;
//     console.log('Scanned Code:', result);
//   }

//   onCamerasFound(devices: MediaDeviceInfo[]): void {
//   console.log('Cameras found:', devices);
// }

// onScanError(error: any): void {
//   console.error('Scan error:', error);
// }

// onScanFailure(error: any): void {
//   console.warn('Scan failed:', error);
// }

// onPermissionResponse(hasPermission: boolean): void {
//   console.log('Permission granted:', hasPermission);
// }
// }
