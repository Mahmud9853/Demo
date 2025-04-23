import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrdersComponent } from './orders.component';
import { OrdersRoutingModule } from './orders-routing.module';
import { CheckoutModule } from '../checkout/checkout.module';
import { OrderDetailedComponent } from '../order-detailed/order-detailed.component';



@NgModule({
  declarations: [
    OrdersComponent,
    
  ],
  imports: [
    CommonModule,
    OrdersRoutingModule,
  ]
})
export class OrdersModule { }
