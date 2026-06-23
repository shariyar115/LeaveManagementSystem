import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '../api.config';
import { LeaveBalanceSummary, Settlement, SettlementHistory } from '../models/leave.models';

@Injectable({ providedIn: 'root' })
export class BalanceService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = inject(API_BASE_URL);

  getSummary(employeeId: number): Observable<LeaveBalanceSummary> {
    const params = new HttpParams().set('employeeId', employeeId);
    return this.http.get<LeaveBalanceSummary>(`${this.baseUrl}/leavebalances`, { params });
  }

  adjust(dto: Settlement): Observable<unknown> {
    return this.http.post(`${this.baseUrl}/settlements`, dto);
  }

  getSettlementHistory(employeeId?: number): Observable<SettlementHistory[]> {
    let params = new HttpParams();
    if (employeeId != null) params = params.set('employeeId', employeeId);
    return this.http.get<SettlementHistory[]>(`${this.baseUrl}/settlements`, { params });
  }
}
