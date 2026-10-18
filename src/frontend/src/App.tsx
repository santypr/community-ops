import { useEffect, useState } from 'react'
import type { EventRepository } from './application/EventRepository'
import { listEvents } from './application/EventRepository'
import type { CommunityEvent } from './domain/CommunityEvent'

type State =
  | { status: 'loading' }
  | { status: 'success'; events: CommunityEvent[] }
  | { status: 'error' }

const dateFormatter = new Intl.DateTimeFormat('es', {
  dateStyle: 'long', timeStyle: 'short',
})

export default function App({ repository }: { repository: EventRepository }) {
  const [state, setState] = useState<State>({ status: 'loading' })
  const [attempt, setAttempt] = useState(0)

  useEffect(() => {
    const controller = new AbortController()
    listEvents(repository, controller.signal)
      .then((events) => {
        if (!controller.signal.aborted) setState({ status: 'success', events })
      })
      .catch(() => {
        if (!controller.signal.aborted) setState({ status: 'error' })
      })
    return () => controller.abort()
  }, [repository, attempt])

  return (
    <main>
      <header className="brand">community<span>ops</span></header>
      <section className="intro" aria-labelledby="page-title">
        <p className="eyebrow">APRENDE · COMPARTE · CONECTA</p>
        <h1 id="page-title">Las comunidades crecen<br />cuando nos encontramos.</h1>
        <p>Descubre los encuentros de tu comunidad técnica.</p>
      </section>
      <section aria-labelledby="events-title">
        <div className="section-heading">
          <h2 id="events-title">Agenda de eventos</h2>
          <span className="badge">Comunidad técnica</span>
        </div>
        <div aria-live="polite" aria-busy={state.status === 'loading'}>
          {state.status === 'loading' && <p role="status">Cargando eventos…</p>}
          {state.status === 'error' && (
            <div role="alert" className="notice">
              <p>No se pudieron cargar los eventos. Inténtalo de nuevo.</p>
              <button onClick={() => {
                setState({ status: 'loading' })
                setAttempt((value) => value + 1)
              }}>Reintentar</button>
            </div>
          )}
          {state.status === 'success' && state.events.length === 0 && (
            <p className="notice">Todavía no hay eventos publicados. ¡Vuelve pronto!</p>
          )}
          {state.status === 'success' && (
            <ul className="events">
              {state.events.map((event) => (
                <li key={event.id}>
                  <article>
                    <p className="eyebrow">{event.communityId}</p>
                    <h3>{event.title}</h3>
                    <p><time dateTime={event.startsAt}>{dateFormatter.format(new Date(event.startsAt))}</time></p>
                    <p>Fin: <time dateTime={event.endsAt}>{dateFormatter.format(new Date(event.endsAt))}</time></p>
                    <p className="location">{event.location}</p>
                  </article>
                </li>
              ))}
            </ul>
          )}
        </div>
      </section>
      <footer>Community Ops · Construyendo comunidad, encuentro a encuentro.</footer>
    </main>
  )
}
