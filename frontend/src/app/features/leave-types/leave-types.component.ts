import { DecimalPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { TooltipModule } from 'primeng/tooltip';

import { LeaveTypeService } from '../../core/services/leave-type.service';
import { LeaveType } from '../../core/models/leave.models';

@Component({
  selector: 'app-leave-types',
  imports: [
    DecimalPipe,
    ReactiveFormsModule,
    TableModule,
    TagModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    InputNumberModule,
    ToggleSwitchModule,
    TooltipModule,
  ],
  templateUrl: './leave-types.component.html',
})
export class LeaveTypesComponent {
  private readonly fb = inject(FormBuilder);
  private readonly leaveTypeService = inject(LeaveTypeService);
  private readonly messageService = inject(MessageService);

  readonly types = signal<LeaveType[]>([]);
  readonly loading = signal(false);
  readonly dialogVisible = signal(false);
  readonly editingId = signal<number | null>(null);
  readonly saving = signal(false);

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    defaultDays: [0, [Validators.required, Validators.min(0), Validators.max(366)]],
    isAccrued: [false],
    accrualRatePerMonth: [null as number | null],
  });

  constructor() {
    this.reload();
  }

  reload(): void {
    this.loading.set(true);
    this.leaveTypeService.getAll().subscribe({
      next: (data) => {
        this.types.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  openCreate(): void {
    this.editingId.set(null);
    this.form.reset({ name: '', defaultDays: 0, isAccrued: false, accrualRatePerMonth: null });
    this.dialogVisible.set(true);
  }

  openEdit(type: LeaveType): void {
    this.editingId.set(type.id);
    this.form.reset({
      name: type.name,
      defaultDays: type.defaultDays,
      isAccrued: type.isAccrued,
      accrualRatePerMonth: type.accrualRatePerMonth ?? null,
    });
    this.dialogVisible.set(true);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const dto = this.form.getRawValue();
    this.saving.set(true);

    const id = this.editingId();
    const request$ = id == null
      ? this.leaveTypeService.create(dto)
      : this.leaveTypeService.update(id, dto);

    request$.subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: id == null ? 'Created' : 'Updated',
          detail: `Leave type "${dto.name}" saved.`,
        });
        this.dialogVisible.set(false);
        this.saving.set(false);
        this.reload();
      },
      error: () => this.saving.set(false),
    });
  }

  remove(type: LeaveType): void {
    this.leaveTypeService.delete(type.id).subscribe(() => {
      this.messageService.add({
        severity: 'success',
        summary: 'Deleted',
        detail: `"${type.name}" removed.`,
      });
      this.reload();
    });
  }
}
