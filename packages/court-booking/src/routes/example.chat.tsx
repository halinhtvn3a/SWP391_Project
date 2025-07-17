import { createFileRoute } from '@tanstack/react-router'
import { useEffect, useRef } from 'react'
import { Send } from 'lucide-react'
import ReactMarkdown from 'react-markdown'
import rehypeRaw from 'rehype-raw'
import rehypeSanitize from 'rehype-sanitize'
import rehypeHighlight from 'rehype-highlight'
import remarkGfm from 'remark-gfm'
import { useChat } from '@ai-sdk/react'

import { genAIResponse } from '../utils/demo.ai'

import type { UIMessage } from 'ai'

import '../demo.index.css'

function InitalLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="flex-1 flex items-center justify-center px-4">
      <div className="text-center max-w-3xl mx-auto w-full">
        <h1 className="text-6xl font-bold mb-4 bg-gradient-to-r from-orange-500 to-red-600 text-transparent bg-clip-text uppercase">
          <span className="text-white">TanStack</span> Chat
        </h1>
        <p className="text-gray-400 mb-6 w-2/3 mx-auto text-lg">
          You can ask me about anything, I might or might not have a good
          answer, but you can still ask.
        </p>
        {children}
      </div>
    </div>
  )
}

function ChattingLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="absolute bottom-0 right-0 left-64 bg-gray-900/80 backdrop-blur-sm border-t border-orange-500/10">
      <div className="max-w-3xl mx-auto w-full px-4 py-3">{children}</div>
    </div>
  )
}

function Messages({ messages }: { messages: Array<UIMessage> }) {
  const messagesContainerRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (messagesContainerRef.current) {
      messagesContainerRef.current.scrollTop =
        messagesContainerRef.current.scrollHeight
    }
  }, [messages])

  if (!messages.length) {
    return null
  }

  return (
    <div ref={messagesContainerRef} className="flex-1 overflow-y-auto pb-24">
      <div className="max-w-3xl mx-auto w-full px-4">
        {messages.map(({ id, role, content }) => (
          <div
            key={id}
            className={`py-6 ${
              role === 'assistant'
                ? 'bg-gradient-to-r from-orange-500/5 to-red-600/5'
                : 'bg-transparent'
            }`}
          >
            <div className="flex items-start gap-4 max-w-3xl mx-auto w-full">
              {role === 'assistant' ? (
                <div className="w-8 h-8 rounded-lg bg-gradient-to-r from-orange-500 to-red-600 mt-2 flex items-center justify-center text-sm font-medium text-white flex-shrink-0">
                  AI
                </div>
              ) : (
                <div className="w-8 h-8 rounded-lg bg-gray-700 flex items-center justify-center text-sm font-medium text-white flex-shrink-0">
                  Y
                </div>
              )}
              <div className="flex-1 min-w-0">
                <ReactMarkdown
                  className="prose dark:prose-invert max-w-none"
                  rehypePlugins={[
                    rehypeRaw,
                    rehypeSanitize,
                    rehypeHighlight,
                    remarkGfm,
                  ]}
                >
                  {content}
                </ReactMarkdown>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

function ChatPage() {
  const { messages, input, handleInputChange, handleSubmit } = useChat({
    initialMessages: [],
    fetch: (_url, options) => {
      const { messages } = JSON.parse(options!.body! as string)
      return genAIResponse({
        data: {
          messages,
        },
      })
    },
  })

  const Layout = messages.length ? ChattingLayout : InitalLayout

  return (
    <div className="relative flex h-[calc(100vh-32px)] bg-gray-900">
      <div className="flex-1 flex flex-col">
        <Messages messages={messages} />

        <Layout>
          <form onSubmit={handleSubmit}>
            <div className="relative max-w-xl mx-auto">
              <textarea
                value={input}
                onChange={handleInputChange}
                placeholder="Type something clever (or don't, we won't judge)..."
                className="w-full rounded-lg border border-orange-500/20 bg-gray-800/50 pl-4 pr-12 py-3 text-sm text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-orange-500/50 focus:border-transparent resize-none overflow-hidden shadow-lg"
                rows={1}
                style={{ minHeight: '44px', maxHeight: '200px' }}
                onInput={(e) => {
                  const target = e.target as HTMLTextAreaElement
                  target.style.height = 'auto'
                  target.style.height =
                    Math.min(target.scrollHeight, 200) + 'px'
                }}
                onKeyDown={(e) => {
                  if (e.key === 'Enter' && !e.shiftKey) {
                    e.preventDefault()
                    handleSubmit(e)
                  }
                }}
              />
              <button
                type="submit"
                disabled={!input.trim()}
                className="absolute right-2 top-1/2 -translate-y-1/2 p-2 text-orange-500 hover:text-orange-400 disabled:text-gray-500 transition-colors focus:outline-none"
              >
                <Send className="w-4 h-4" />
              </button>
            </div>
          </form>
        </Layout>
      </div>
    </div>
  )
}

export const Route = createFileRoute('/example/chat')({
  component: ChatPage,
})
