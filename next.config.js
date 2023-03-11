/** @type {import('next').NextConfig} */
const nextConfig = {
  experimental: {
    appDir: true,
  },
  output: "standalone",
  async redirects() {
    return [
      {
        source: '/',
        destination: '/flowtime',
        permanent: false,
      }]
  }
}

module.exports = nextConfig
