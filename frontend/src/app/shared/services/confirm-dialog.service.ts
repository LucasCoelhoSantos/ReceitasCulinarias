import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface ConfirmDialogData {
  title: string;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class ConfirmDialogService {
  private confirmSubject = new Subject<boolean>();
  private dialogData = new Subject<ConfirmDialogData>();

  showDialog(data: ConfirmDialogData): Promise<boolean> {
    this.dialogData.next(data);
    return new Promise<boolean>((resolve) => {
      const subscription = this.confirmSubject.subscribe((result) => {
        subscription.unsubscribe();
        resolve(result);
      });
    });
  }

  confirm() {
    this.confirmSubject.next(true);
  }

  cancel() {
    this.confirmSubject.next(false);
  }

  getDialogData() {
    return this.dialogData.asObservable();
  }
} 