import assert from 'node:assert/strict'
import { test } from 'node:test'
import { HttpEventRepository } from '../src/infrastructure/HttpEventRepository.ts'
import { listEvents } from '../src/application/EventRepository.ts'

const event = {
  id: 'e4a9953c-3cd9-428e-aeb3-f87eb2e87837',
  communityId: 'community-ops',
  title: 'Encuentro',
  startsAt: '2030-01-15T18:00:00Z',
  endsAt: '2030-01-15T19:00:00Z',
  location: 'Online',
}

test('HTTP adapter uses configured URL and forwards cancellation', async (context) => {
  const signal = new AbortController().signal
  context.mock.method(globalThis, 'fetch', async (url, options) => {
    assert.equal(url, 'https://api.example/api/events')
    assert.equal(options.signal, signal)
    return Response.json([event])
  })
  assert.deepEqual(await new HttpEventRepository('https://api.example/').list(signal), [event])
})

test('HTTP adapter accepts an empty catalog', async (context) => {
  context.mock.method(globalThis, 'fetch', async () => Response.json([]))
  assert.deepEqual(await new HttpEventRepository('').list(new AbortController().signal), [])
})

test('HTTP adapter rejects failed requests', async (context) => {
  context.mock.method(globalThis, 'fetch', async () => new Response(null, { status: 503 }))
  await assert.rejects(new HttpEventRepository('').list(new AbortController().signal))
})

for (const data of [{ items: [] }, [{ ...event, startsAt: 'invalid' }], [{ title: 'missing fields' }]]) {
  test(`HTTP adapter rejects malformed data ${JSON.stringify(data)}`, async (context) => {
    context.mock.method(globalThis, 'fetch', async () => Response.json(data))
    await assert.rejects(new HttpEventRepository('').list(new AbortController().signal))
  })
}

test('use case delegates to the injected repository', async () => {
  const signal = new AbortController().signal
  const repository = {
    async list(receivedSignal) {
      assert.equal(receivedSignal, signal)
      return [event]
    },
  }
  assert.deepEqual(await listEvents(repository, signal), [event])
})
