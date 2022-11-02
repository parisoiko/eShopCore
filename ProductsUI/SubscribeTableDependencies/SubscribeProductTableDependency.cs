﻿namespace ProductsUI.SubscribeTableDependencies
{
    public class SubscribeProductTableDependency : ISubscribeTableDependency
    {
        SqlTableDependency<Products> tableDependency;
        DashboardHub dashboardHub;

        public SubscribeProductTableDependency(DashboardHub dashboardHub) {
            this.dashboardHub = dashboardHub;
        }

        public void SubscribeTableDependency(string connectionString) {
            tableDependency = new SqlTableDependency<Products>(connectionString);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();
        }

        private void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Products> e) {
            if (e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None) {
                dashboardHub.SendProducts();
            }
        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e) {
            Console.WriteLine($"{nameof(Products)} SqlTableDependency error: {e.Error.Message}");
        }
    }
}
