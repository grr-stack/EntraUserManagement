import { useEffect, useState } from 'react';
import StatCard from '../components/StatCard.jsx';
import { getDashboardData } from '../lib/mockApi.js';

function DashboardPage() {
  const [dashboard, setDashboard] = useState({
    stats: [],
    recentUsers: [],
    recentInventory: [],
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let active = true;

    getDashboardData().then((data) => {
      if (active) {
        setDashboard(data);
        setLoading(false);
      }
    });

    return () => {
      active = false;
    };
  }, []);

  if (loading) {
    return <div className="loading-panel">Refreshing dashboard metrics...</div>;
  }

  return (
    <div className="page-stack">
      <section className="hero-panel">
        <div>
          <p className="eyebrow">Overview</p>
          <h3>gppSee users, stock, and activity at a glance.</h3>
          <p>
            Each section below pulls from the mock API layer so the dashboard stays synced
            with user and inventory changes.
          </p>
        </div>
      </section>

      <section className="stats-grid">
        {dashboard.stats.map((stat) => (
          <StatCard key={stat.label} {...stat} />
        ))}
      </section>

      <section className="grid-two">
        <article className="data-card">
          <div className="section-heading">
            <div>
              <p className="eyebrow">Recent users</p>
              <h3>Latest registrations</h3>
            </div>
          </div>
          <div className="list-stack">
            {dashboard.recentUsers.map((user) => (
              <div key={user.id} className="list-row">
                <div>
                  <strong>{user.name}</strong>
                  <span>{user.email}</span>
                </div>
                <span className={`tag ${user.status.toLowerCase()}`}>{user.status}</span>
              </div>
            ))}
          </div>
        </article>

        <article className="data-card">
          <div className="section-heading">
            <div>
              <p className="eyebrow">Inventory pulse</p>
              <h3>Newest stock items</h3>
            </div>
          </div>
          <div className="list-stack">
            {dashboard.recentInventory.map((item) => (
              <div key={item.id} className="list-row">
                <div>
                  <strong>{item.itemName}</strong>
                  <span>{item.category}</span>
                </div>
                <span className="metric-pill">{item.quantity} units</span>
              </div>
            ))}
          </div>
        </article>
      </section>
    </div>
  );
}

export default DashboardPage;
