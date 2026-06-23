import { Injectable, computed, inject, signal } from '@angular/core';
import { EmployeeService } from './employee.service';
import { Employee } from '../models/leave.models';

/**
 * Holds app-wide selection state. The spec assumes single-user access, but a
 * selectable employee makes the approval / HR features demonstrable.
 */
@Injectable({ providedIn: 'root' })
export class AppStateService {
  private readonly employeeService = inject(EmployeeService);

  readonly employees = signal<Employee[]>([]);
  readonly selectedEmployeeId = signal<number>(1);

  readonly selectedEmployee = computed(() =>
    this.employees().find((e) => e.id === this.selectedEmployeeId()) ?? null,
  );

  loadEmployees(): void {
    this.employeeService.getAll().subscribe((list) => {
      this.employees.set(list);
      if (list.length && !list.some((e) => e.id === this.selectedEmployeeId())) {
        this.selectedEmployeeId.set(list[0].id);
      }
    });
  }

  selectEmployee(id: number): void {
    this.selectedEmployeeId.set(id);
  }
}
