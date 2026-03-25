/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      colors: {
        ink: {
          50: '#f7f7f9',
          100: '#efeff4',
          200: '#d8dae4',
          300: '#b6b9c9',
          400: '#8b90a8',
          500: '#686e86',
          600: '#4f556d',
          700: '#3c4054',
          800: '#262837',
          900: '#141521',
          950: '#0b0c13',
        },
      },
      boxShadow: {
        lift: '0 14px 28px rgba(11,12,19,0.12), 0 3px 8px rgba(11,12,19,0.10)',
      },
    },
  },
  plugins: [],
}

