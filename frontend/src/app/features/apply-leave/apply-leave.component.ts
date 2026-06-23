import { Component, computed, effect, inject, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router } from '@angular/router';
import { debounceTime } from 'rxjs';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';
import { MessageModule } from 'primeng/message';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';

import { BalanceService } from '../../core/services/balance.service';
import { LeaveRequestService } from '../../core/services/leave-request.service';
import { LeaveTypeService } from '../../core/services/leave-type.service';
import { AppStateService } from '../../core/services/app-state.service';
import {
  LeaveBalance,
  LeaveBalanceSummary,
  LeaveType,
} from '../../core/models/leave.models';
import { businessDaysBetween } from '../../shared/date.util';

@Component({
  selector: 'app-apply-leave',
  imports: [
    ReactiveFormsModule,
    SelectModule,
    DatePickerModule,
    TextareaModule,
    ButtonModule,
    MessageModule,
  ],
  templateUrl: './apply-leave.component.html',
})
export class ApplyLeaveComponent {
  private readonly fb = inject(FormBuilder);
  private readonly leaveRequestService = inject(LeaveRequestService);
  private readonly leaveTypeService = inject(LeaveTypeService);
  private readonly balanceService = inject(BalanceService);
  private readonly messageService = inject(MessageService);
  private readonly router = inject(Router);
  readonly state = inject(AppStateService);

  readonly leaveTypes = signal<LeaveType[]>([]);
  readonly summary = signal<LeaveBalanceSummary | null>(null);
  readonly submitting = signal(false);

  readonly minDate = new Date();

  readonly form = this.fb.nonNullable.group({
    leaveTypeId: [null as number | null, Validators.required],
    startDate: [null as Date | null, Validators.required],
    endDate: [null as Date | null, Validators.required],
    reason: [''],
  });

  // Reactive view of the form so computed signals can react to changes.
  private readonly formValue = signal(this.form.getRawValue());

  readonly estimatedDays = computed(() => {
    const { startDate, endDate } = this.formValue();
    if (!startDate || !endDate) return 0;
    return businessDaysBetween(startDate, endDate);
  });

  readonly selectedBalance = computed<LeaveBalance | null>(() => {
    const id = this.formValue().leaveTypeId;
    if (id == null) return null;
    return this.summary()?.balances.find((b) => b.leaveTypeId === id) ?? null;
  });

  readonly dateOrderInvalid = computed(() => {
    const { startDate, endDate } = this.formValue();
    return !!startDate && !!endDate && startDate > endDate;
  });

  readonly insufficientBalance = computed(() => {
    const bal = this.selectedBalance();
    return bal != null && this.estimatedDays() > 0 && this.estimatedDays() > bal.balance;
  });

  readonly lowBalanceWarning = computed(() => {
    const bal = this.selectedBalance();
    if (!bal || this.estimatedDays() <= 0 || this.insufficientBalance()) return false;
    return bal.balance - this.estimatedDays() <= 2; // warn when little is left
  });

  private get draftKey(): string {
    return `leave-draft-emp-${this.state.selectedEmployeeId()}`;
  }

  constructor() {
    this.leaveTypeService.getAll().subscribe((types) => this.leaveTypes.set(types));

    // Keep the reactive signal in sync + persist a draft (debounced).
    this.form.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.formValue.set(this.form.getRawValue()));

    this.form.valueChanges
      .pipe(debounceTime(400), takeUntilDestroyed())
      .subscribe(() => this.saveDraft());

    // Load balances + restore any saved draft whenever the employee changes.
    effect(() => {
      const employeeId = this.state.selectedEmployeeId();
      this.balanceService.getSummary(employeeId).subscribe((s) => this.summary.set(s));
      this.restoreDraft();
    });
  }

  private saveDraft(): void {
    if (this.form.pristine) return;
    localStorage.setItem(this.draftKey, JSON.stringify(this.form.getRawValue()));
  }

  private restoreDraft(): void {
    const raw = localStorage.getItem(this.draftKey);
    if (!raw) {
      this.form.reset();
      this.formValue.set(this.form.getRawValue());
      return;
    }
    try {
      const draft = JSON.parse(raw);
      this.form.patchValue({
        leaveTypeId: draft.leaveTypeId ?? null,
        startDate: draft.startDate ? new Date(draft.startDate) : null,
        endDate: draft.endDate ? new Date(draft.endDate) : null,
        reason: draft.reason ?? '',
      });
      this.formValue.set(this.form.getRawValue());
    } catch {
      localStorage.removeItem(this.draftKey);
    }
  }

  clearDraft(): void {
    localStorage.removeItem(this.draftKey);
    this.form.reset();
    this.formValue.set(this.form.getRawValue());
    this.messageService.add({ severity: 'info', summary: 'Draft cleared' });
  }

  hasDraft(): boolean {
    return !!localStorage.getItem(this.draftKey);
  }

  private toIsoDate(date: Date): string {
    // Send a date-only string to avoid timezone shifting.
    const y = date.getFullYear();
    const m = `${date.getMonth() + 1}`.padStart(2, '0');
    const d = `${date.getDate()}`.padStart(2, '0');
    return `${y}-${m}-${d}`;
  }

  submit(): void {
    if (this.form.invalid || this.dateOrderInvalid() || this.insufficientBalance()) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    this.submitting.set(true);
    this.leaveRequestService
      .submit({
        employeeId: this.state.selectedEmployeeId(),
        leaveTypeId: value.leaveTypeId!,
        startDate: this.toIsoDate(value.startDate!),
        endDate: this.toIsoDate(value.endDate!),
        reason: value.reason || null,
      })
      .subscribe({
        next: () => {
          this.submitting.set(false);
          this.messageService.add({
            severity: 'success',
            summary: 'Submitted',
            detail: 'Leave request submitted successfully.',
          });
          localStorage.removeItem(this.draftKey);
          this.router.navigate(['/dashboard']);
        },
        error: () => this.submitting.set(false),
      });
  }
}
