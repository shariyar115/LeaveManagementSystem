import { DatePipe } from '@angular/common';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';

import { BalanceService } from '../../core/services/balance.service';
import { AppStateService } from '../../core/services/app-state.service';
import {
  LeaveBalanceSummary,
  SettlementHistory,
} from '../../core/models/leave.models';

@Component({
  selector: 'app-settlements',
  imports: [
    DatePipe,
    ReactiveFormsModule,
    SelectModule,
    InputNumberModule,
    InputTextModule,
    TableModule,
    ButtonModule,
  ],
  templateUrl: './settlements.component.html',
})
export class SettlementsComponent {
  private readonly fb = inject(FormBuilder);
  private readonly balanceService = inject(BalanceService);
  private readonly messageService = inject(MessageService);
  readonly state = inject(AppStateService);

  readonly summary = signal<LeaveBalanceSummary | null>(null);
  readonly history = signal<SettlementHistory[]>([]);
  readonly saving = signal(false);

  readonly form = this.fb.nonNullable.group({
    leaveTypeId: [null as number | null, Validators.required],
    newBalance: [0, [Validators.required, Validators.min(0)]],
    reason: [''],
  });

  readonly currentBalance = computed(() => {
    const id = this.form.controls.leaveTypeId.value;
    return this.summary()?.balances.find((b) => b.leaveTypeId === id) ?? null;
  });

  constructor() {
    effect(() => {
      const employeeId = this.state.selectedEmployeeId();
      this.loadSummary(employeeId);
      this.loadHistory(employeeId);
      this.form.reset({ leaveTypeId: null, newBalance: 0, reason: '' });
    });
  }

  private loadSummary(employeeId: number): void {
    this.balanceService.getSummary(employeeId).subscribe((s) => this.summary.set(s));
  }

  private loadHistory(employeeId: number): void {
    this.balanceService.getSettlementHistory(employeeId).subscribe((h) => this.history.set(h));
  }

  prefillCurrent(): void {
    const bal = this.currentBalance();
    if (bal) this.form.controls.newBalance.setValue(bal.balance);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.saving.set(true);
    this.balanceService
      .adjust({
        employeeId: this.state.selectedEmployeeId(),
        leaveTypeId: value.leaveTypeId!,
        newBalance: value.newBalance,
        reason: value.reason || null,
      })
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Balance adjusted',
            detail: 'Settlement applied successfully.',
          });
          this.saving.set(false);
          const employeeId = this.state.selectedEmployeeId();
          this.loadSummary(employeeId);
          this.loadHistory(employeeId);
          this.form.controls.reason.setValue('');
        },
        error: () => this.saving.set(false),
      });
  }
}
