import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
	title: 'PetWeb',
	description: 'Pet adoption frontend rebuilt with Next.js',
};

export default function RootLayout({
	children,
}: Readonly<{ children: React.ReactNode }>) {
	return (
		<html lang="en">
			<body className="min-h-screen bg-gray-50 text-gray-900 antialiased">
				{children}
			</body>
		</html>
	);
}